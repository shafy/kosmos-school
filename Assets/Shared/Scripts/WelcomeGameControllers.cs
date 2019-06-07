using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // controls for the welcome scene
  public class WelcomeGameControllers : MonoBehaviour {


    void Update() {
      if (OVRInput.GetDown(OVRInput.Button.Back)) {
        // minimize app
        OVRManager.PlatformUIConfirmQuit();
      }
    }
  }
}
