using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos.Shared;

namespace Kosmos
{
  // The Electromagnetic Grabber can grab objects from a distance
  public class ElectroGrabber : MonoBehaviour {

    private bool isGrabbing;
    private bool lineRendererActive;
    private ConstrainPosition constrainPosition;
    private Grabbable currentGrabbable;
    private float DistanceToObj;
    private float currentTouchPadY;
    private float prevTouchPadY;
    private Matrix4x4 localToWorld;
    private Quaternion controllerOrientation;
    private ParticleSystem.MainModule pMain;
    private ParticleSystem.MinMaxCurve psInitialStartSize;
    private SliderControl sliderControl;
    private Transform trackingSpace;
    private Rigidbody currentGrabbableRb;

    [SerializeField] private ControllerRayCaster controllerRayCaster;
    [SerializeField] private GameObject destinationPS;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private PlayerController playerController;
    
    void Start() {
      // get tracking space
      trackingSpace = GameObject.FindWithTag("TrackingSpace").transform;

      isGrabbing = false;

      DistanceToObj = 0.0f;
      resetTouchPadY();

      // get particle system's main module
      pMain = particleSystem.main;
      // save default lifetime from particle system so we can go back to it later
      psInitialStartSize = pMain.startSize;

      lineRendererActive = true;

      setDestinationPS(false);
    }

    public void Update() {
      // stop executing Update if playerController is Frozen
      if (playerController.Frozen) {
        setDestinationPS(true);
        return;
      }
      
      // stop execution here if Quest
      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") return;

      // if player is walking, always disable linerender
      // if (lineRendererActive && playerController.IsWalking) {
      //   enabledRayCaster(false);
      // } else if (!lineRendererActive && !playerController.IsWalking) {
      //   enabledRayCaster(true);
      // }
     

      if (!playerController.IsWalking && controllerRayCaster.CurrentInteractible) {
        setDestinationPS(true);
      } else {
        setDestinationPS(false);
      }

      // only triggered once one player presses the trigger down
      if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && controllerRayCaster.CurrentInteractible) {
        // get current object targeted by raycast
        InteractiveItem currentInteractible = controllerRayCaster.CurrentInteractible;

        // get it's button component
        Button currentButton = currentInteractible.gameObject.GetComponent<Button>();
        if (currentButton == null) return;

        // press the button
        currentButton.Press();
      }

      // play presses trigger button: grab item
      if (!isGrabbing && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && controllerRayCaster.CurrentInteractible) {
        grabItem();
      }

      // player let's go of triger button: ungrab item
      if (isGrabbing && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
        ungrabItem();
      }

      if (isGrabbing) {
        // ungrabItem if it becomes ungrabbable
        if (!currentGrabbable.IsGrabbable) {
          ungrabItem();
          return;
        }
        // move object with ElectroGrabber
        moveItem();

        // change item distance from player (only if not walking)
        if (playerController.IsWalking) {
          resetTouchPadY();
        } else {
          changeItemDistance();
        }
        // show laser beam to object
        extendLaser();
      }
    }

    private void grabItem() {
      // get current object targeted by raycast
      InteractiveItem currentInteractible = controllerRayCaster.CurrentInteractible;

      // get it's Grabbable component
      currentGrabbable = currentInteractible.gameObject.GetComponent<Grabbable>();
      if (currentGrabbable == null) return;

      // don't grab it if it has IsGrabbable = false;
      if (!currentGrabbable.IsGrabbable) return;

      // make kinematic
      currentGrabbableRb = currentGrabbable.gameObject.GetComponent<Rigidbody>();
      currentGrabbableRb.isKinematic = true;

      // calculate distance to obj
      Vector3 controllerPos = OVRInput.GetLocalControllerPosition(KosmosStatics.Controller);
      localToWorld = trackingSpace.localToWorldMatrix;
      Vector3 controllerPosWorld = localToWorld.MultiplyPoint(controllerPos);
      Vector3 objectPos = currentGrabbable.transform.position;

      DistanceToObj = Vector3.Distance(controllerPosWorld, objectPos);

      isGrabbing = true;

      currentGrabbable.Grabbed();

      // check if it has a ConstrainPosition component
      constrainPosition = currentInteractible.gameObject.GetComponent<ConstrainPosition>();

      // check if it has a SliderControl component
      sliderControl = currentInteractible.gameObject.GetComponent<SliderControl>();
    }

    private void ungrabItem() {
      isGrabbing = false;
      if (currentGrabbable == null) return;

      currentGrabbableRb.isKinematic = false;

      resetLaser();
      currentGrabbable.Ungrabbed();

      constrainPosition = null;
      sliderControl = null;
    }

    private void moveItem() {
      Vector3 controllerPos = OVRInput.GetLocalControllerPosition(KosmosStatics.Controller);
      controllerOrientation = OVRInput.GetLocalControllerRotation(KosmosStatics.Controller);

      // calculate point where object goes based on fixed distance from controller
      Vector3 targetPos = controllerPos + ((controllerOrientation * Vector3.forward) * DistanceToObj);

      // convert target position to world space
      localToWorld = trackingSpace.localToWorldMatrix;
      Vector3 targetPosWorld = localToWorld.MultiplyPoint(targetPos);

      Vector3 newPos = Vector3.Lerp(currentGrabbable.transform.position, targetPosWorld, Time.deltaTime * 10);

      // phantom position means that the real position is controlled by something else but we still want to set
      // the position it would have been
      // phantom position can e.g. be used if grabbing an end of a cable and the cable can't be stretched
      if (currentGrabbable.IsPhantom) {
        currentGrabbable.PhantomPosition = newPos;
      } else {
        currentGrabbable.transform.position = newPos;
      }

      // if it is being constrained, get position from the ConstrainPosition component
      if (constrainPosition) {
        currentGrabbable.transform.localPosition = constrainPosition.Constrain();
      }

      // if it has a slidercontrol, make sure to add additional constraints
      if (sliderControl) {
        currentGrabbable.transform.localPosition = sliderControl.Constrain();
      }
      
    }

    private void changeItemDistance() {
      // if not touching, reset values and return
      if (!OVRInput.Get(OVRInput.Touch.PrimaryTouchpad)) {
        resetTouchPadY();
        return;
      }

      currentTouchPadY = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;

      // change distance to object based on y pos of touchpad
      float distanceToObjDelta = currentTouchPadY - prevTouchPadY;
      if (prevTouchPadY != 0.0f && Mathf.Abs(distanceToObjDelta) > 0.01f) {
        float newDistance = DistanceToObj + distanceToObjDelta;
        // only update DistanceToObj if not to close or far away
        if (newDistance > 0.3f && newDistance < 6.0f) {
          DistanceToObj = newDistance;
        }
      }
      prevTouchPadY = currentTouchPadY;
    }

    private void resetTouchPadY() {
      currentTouchPadY = 0.0f;
      prevTouchPadY = 0.0f;
    }

    private void extendLaser() {
      ParticleSystem.MinMaxCurve rate = new ParticleSystem.MinMaxCurve();
      rate.constantMax = 0.1f;
      pMain.startSize = rate;
    }

    private void resetLaser() {
      pMain.startSize = psInitialStartSize;
    }

    private void enabledRayCaster(bool enabled) {
      controllerRayCaster.RayCastEnabled = enabled;
      controllerRayCaster.EnableLineRenderer(enabled); 
      lineRendererActive = enabled;
    }

    private void setDestinationPS(bool value) {
      if (value) {
        destinationPS.active = true;
        destinationPS.transform.position = controllerRayCaster.CurrentHit.point;
      } else {
        destinationPS.active = false;
      }
      
    }
  }
}

