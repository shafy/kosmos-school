using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // all logic related to player controll and movement
  public class PlayerController : OVRPlayerController {
   
    private bool isWalking;
    private CharacterController character;
    private GameObject centerEyeAnchor;
    private Quaternion controllerRotation;
    private Vector2 primayTouchpadPos;

    
    public bool WalkingAllowed = true;
  
    public bool IsWalking {
      get { return isWalking; }
      private set { isWalking = value; }
    }

    void Start () {
      character = GetComponent<CharacterController>();
      centerEyeAnchor = GameObject.FindWithTag("MainCamera");
      IsWalking = false;
    }
    
    // override OVRPlayerController's Update
    void Update () {
      // only move if touchpad is pressed and walking is not disabled
      if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad) && WalkingAllowed) {
        //controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
        controllerRotation = KosmosStatics.ControllerOrientation();

        primayTouchpadPos = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

        Vector3 movementDirection = new Vector3(primayTouchpadPos.x, 0.0f, primayTouchpadPos.y);
        Vector3 movement = controllerRotation * movementDirection;
       
        character.SimpleMove(movement * 5);

        IsWalking = true;
      } else {
        IsWalking = false;
      }
    }

    // enables straiht physics raycaster
    public void EnableRay(bool enable) {
      centerEyeAnchor.GetComponent<ControllerRayCaster>().ShowLineRenderer = enable;
    }

    // gets hand anchor based on dominant hand
    public GameObject HandAnchor() {
      if (OVRInput.GetDominantHand() == OVRInput.Handedness.RightHanded) {
        return GameObject.FindWithTag("RightHandAnchor");
      } else if (OVRInput.GetDominantHand() == OVRInput.Handedness.LeftHanded) {
        return GameObject.FindWithTag("LeftHandAnchor");
      }

      // default to right hand
      return GameObject.FindWithTag("RightHandAnchor");
    }
  }
}
