using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // all logic related to player controll and movement
  public class PlayerController : OVRPlayerController {
    
    private AudioSource audioSource;
    private bool isWalking;
    private bool frozen;
    private GameObject centerEyeAnchor;
    private Quaternion controllerRotation;
    private Vector2 primayTouchpadPos;
  
    public bool IsWalking {
      get { return isWalking; }
      private set { isWalking = value; }
    }

    public bool Frozen {
      get { return frozen; }
      set { frozen = value; }
    }

    void Start () {
      centerEyeAnchor = GameObject.FindWithTag("MainCamera");
      IsWalking = false;
      frozen = false;
      audioSource = GetComponent<AudioSource>();
    }
    
    // override OVRPlayerController's Update
    void Update () {
      //Debug.Log("UnityEngine.XR.XRDevice.model " + UnityEngine.XR.XRDevice.model);

      // check if player is currently walking
      if (!isWalking && Controller.velocity != Vector3.zero) {
        isWalking = true;
        playWalkingSound(true);
      }

      if (isWalking && Controller.velocity == Vector3.zero) {
        isWalking = false;
        playWalkingSound(false);
      }

      // ** GO ONLY **
      // only move if touchpad is pressed and walking is not disabled
      if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad) && HaltUpdateMovement) {
        controllerRotation = KosmosStatics.ControllerOrientation();

        primayTouchpadPos = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

        Vector3 movementDirection = new Vector3(primayTouchpadPos.x, 0.0f, primayTouchpadPos.y);
        Vector3 movement = controllerRotation * movementDirection;
       
        Controller.SimpleMove(movement * 3);
      }
    }

    private void playWalkingSound(bool enable) {
      if (!audioSource) return;

      if (enable) {
         audioSource.Play();
      } else {
        audioSource.Stop();
      }
    }

    // enables physics raycaster
    public void EnableRay(bool enable) {
      centerEyeAnchor.GetComponent<ControllerRayCaster>().ShowLineRenderer = enable;
    }
  }
}
