using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // general methods used throughout our scripts that don't need to be attached to a specific instance of class
  public class KosmosStatics : MonoBehaviour {

    private Quaternion controllerOrientation;

    public static OVRInput.Controller Controller {
      get {
          OVRInput.Controller controller = OVRInput.GetConnectedControllers();
          if ((controller & OVRInput.Controller.LTrackedRemote) == OVRInput.Controller.LTrackedRemote) {
              return OVRInput.Controller.LTrackedRemote;
          } else if ((controller & OVRInput.Controller.RTrackedRemote) ==  OVRInput.Controller.RTrackedRemote) {
              return OVRInput.Controller.RTrackedRemote;
          }
          return OVRInput.GetActiveController();
      }
    }

    public static Quaternion ControllerOrientation() {
      return OVRInput.GetLocalControllerRotation(Controller);
    }
  }
}