using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
    public class InputSetup : MonoBehaviour {

      private bool controllerFound;
      private bool haltUpdateMovementInitial;
      private ControllerRayCaster controllerRayCaster;
      private GameObject rightHandAnchor;
      private GameObject leftHandAnchor;
      private PlayerController playerController;

      [SerializeField] GameObject errorMessage;

      void Awake() {
        // get hand anchors
        rightHandAnchor = GameObject.FindWithTag("RightHandAnchor");
        leftHandAnchor = GameObject.FindWithTag("LeftHandAnchor");

        controllerRayCaster = GameObject.FindWithTag("MainCamera").GetComponent<ControllerRayCaster>();
        playerController = GameObject.FindWithTag("OVRPlayerController").GetComponent<PlayerController>();
        haltUpdateMovementInitial = playerController.HaltUpdateMovement;
      }

      void Start() {
        controllerFound = false;
        showErrorWindow(false);
      }

      void Update() {
        if (!controllerFound) {
          checkForController();
        }
      }

      void OnApplicationPause(bool isPaused) {
        // when coming back from being paused, re-do controller check
        if (!isPaused) {
          controllerFound = false;
        }
      }

      private void checkForController() {
        OVRInput.Controller currentController = OVRInput.GetConnectedControllers();
        // return if no controller found
        if (currentController == OVRInput.Controller.None) return;

        controllerFound = true;
        showErrorWindow(false);

        if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
          // make sure both controllers are connected
          if (((currentController & OVRInput.Controller.LTouch) == OVRInput.Controller.LTouch) && ((currentController & OVRInput.Controller.RTouch) == OVRInput.Controller.RTouch)) {
            // deactivate both ElectroGrabbers
            rightHandAnchor.transform.Find("RightControllerAnchor").Find("ElectroGrabber").gameObject.SetActive(false);
            leftHandAnchor.transform.Find("LeftControllerAnchor").Find("ElectroGrabber").gameObject.SetActive(false);

            return;
            
          }
        } else {
          // in this case it's a Go or Gear
          // hide Quest hands
          rightHandAnchor.transform.Find("HandRight").gameObject.SetActive(false);
          leftHandAnchor.transform.Find("HandLeft").gameObject.SetActive(false);

          OVRInput.Controller activeController = OVRInput.GetActiveController();
          if (activeController == OVRInput.Controller.LTrackedRemote) {
            rightHandAnchor.SetActive(false);
            leftHandAnchor.SetActive(true);
            return;
          }

          if (activeController == OVRInput.Controller.RTrackedRemote) {
            leftHandAnchor.SetActive(false);
            rightHandAnchor.SetActive(true);
            return;
          }
        }

        // in this case, no Oculus Go remote has been found
        // show error message
        controllerFound = false;
        showErrorWindow(true);
        return;
      }

      private void toggleLine(bool value) {
        controllerRayCaster.RayCastEnabled = value;
        controllerRayCaster.EnableLineRenderer(value);
      }

      private void showErrorWindow(bool value) {

        if (value == true) {
          errorMessage.SetActive(true);

          playerController.HaltUpdateMovement = true;
          // position 2.5 meters in front of user and rotate towards user
          Vector3 newPosError = playerController.transform.position + playerController.transform.forward * 2f;
          newPosError.y = 1.6f;
          errorMessage.transform.position = newPosError;

          Vector3 lookPos = errorMessage.transform.position - playerController.transform.position;
          Quaternion tempRotation = Quaternion.LookRotation(lookPos);
          Quaternion newRotation = Quaternion.Euler(0, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z);
          errorMessage.transform.rotation = newRotation;
          return;
        }

        errorMessage.SetActive(false);
        // only set walking allowed if it was allowed in the first place
        playerController.HaltUpdateMovement = haltUpdateMovementInitial;
        return;
      }

      // gets hand anchor based on dominant hand
      // public GameObject HandAnchor() {
      //   if (OVRInput.GetDominantHand() == OVRInput.Handedness.RightHanded) {
      //     return rightHandAnchor;
      //   } else if (OVRInput.GetDominantHand() == OVRInput.Handedness.LeftHanded) {
      //     return leftHandAnchor;
      //   }

      //   // default to right hand
      //   return rightHandAnchor;
      // }

      // // deactivate other hand
      // private void setOtherHandAnchorInactive() {
      //   if (HandAnchor().CompareTag("RightHandAnchor")) {
      //     leftHandAnchor.SetActive(false);
      //     return;
      //   } else {
      //    rightHandAnchor.SetActive(false);
      //     return;
      //   }
      // }
  }

}