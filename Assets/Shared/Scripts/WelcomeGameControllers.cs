using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // controls for the welcome scene
  public class WelcomeGameControllers : MonoBehaviour {

    [SerializeField] private ControllerRayCaster controllerRayCaster;

    void Start() {
      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
       controllerRayCaster.RayCastEnabled = true;
       controllerRayCaster.EnableLineRenderer(true);
      }
    }

    void Update() {
      // Button.Back is for Go, Button.Start for Quest
      if (OVRInput.GetDown(OVRInput.Button.Back) || OVRInput.GetDown(OVRInput.Button.Start)) {
        // minimize app
        OVRManager.PlatformUIConfirmQuit();
      }
    }
  }
}
