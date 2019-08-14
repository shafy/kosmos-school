using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
    public class InputSetup : MonoBehaviour {

      private bool controllerFound;
      private bool playerWalkingAllowedInitial;
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
        playerWalkingAllowedInitial = playerController.WalkingAllowed;
      }

      void Start() {
        controllerFound = false;
        toggleLine(false);
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
          //checkForController();
        }
      }

      private void checkForController() {
        OVRInput.Controller currentController = OVRInput.GetActiveController();
        // return if no controller found
        if (currentController == OVRInput.Controller.None) return;

        controllerFound = true;
        toggleLine(true);
        showErrorWindow(false);

        // this means we're on the Go or Gear
        if (currentController != OVRInput.Controller.Touch) {
          //if controller found, check if it's the right one
          if (currentController == OVRInput.Controller.LTrackedRemote) {
            rightHandAnchor.SetActive(false);
            leftHandAnchor.SetActive(true);
            return;
          }

          if (currentController == OVRInput.Controller.RTrackedRemote) {
            leftHandAnchor.SetActive(false);
            rightHandAnchor.SetActive(true);
            return;
          }
        } else {
          return;
        }

        // in this case, no Oculus Go remote has been found
        // show error message
        controllerFound = false;
        toggleLine(true);
        showErrorWindow(true);
        return;
      }

      private void toggleLine(bool value) {
        controllerRayCaster.EnableLineRenderer(value);
      }

      private void showErrorWindow(bool value) {

        if (value == true) {
          errorMessage.SetActive(true);

          playerController.WalkingAllowed = false;
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
        if (playerWalkingAllowedInitial) playerController.WalkingAllowed = true;
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