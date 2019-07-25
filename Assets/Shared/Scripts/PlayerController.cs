using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // all logic related to player controll and movement
  public class PlayerController : OVRPlayerController {
    
    private AudioSource audioSource;
    private bool isWalking;
    private bool walkingSoundPlaying;
    private bool frozen;
    private CharacterController character;
    private GameObject centerEyeAnchor;
    private Quaternion controllerRotation;
    private Vector2 primayTouchpadPos;

    
    public bool WalkingAllowed = true;
  
    public bool IsWalking {
      get { return isWalking; }
      private set { isWalking = value; }
    }

    public bool Frozen {
      get { return frozen; }
      set { frozen = value; }
    }

    void Start () {
      character = GetComponent<CharacterController>();
      centerEyeAnchor = GameObject.FindWithTag("MainCamera");
      IsWalking = false;
      walkingSoundPlaying = false;
      frozen = false;
      audioSource = GetComponent<AudioSource>();
    }
    
    // override OVRPlayerController's Update
    void Update () {
      // only move if touchpad is pressed and walking is not disabled
      if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad) && WalkingAllowed) {
        controllerRotation = KosmosStatics.ControllerOrientation();

        primayTouchpadPos = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

        Vector3 movementDirection = new Vector3(primayTouchpadPos.x, 0.0f, primayTouchpadPos.y);
        Vector3 movement = controllerRotation * movementDirection;
       
        character.SimpleMove(movement * 5);

        if (audioSource && !walkingSoundPlaying) {
          audioSource.Play();
          walkingSoundPlaying = true;
        }

        IsWalking = true;

      } else {
        if (audioSource && walkingSoundPlaying) {
          audioSource.Stop();
          walkingSoundPlaying = false;
        }
        IsWalking = false;
      }
    }

    // enables physics raycaster
    public void EnableRay(bool enable) {
      centerEyeAnchor.GetComponent<ControllerRayCaster>().ShowLineRenderer = enable;
    }
  }
}
