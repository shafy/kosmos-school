using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // part of the Placer System. indicators are displayed or hidden based on which Placer ID the
  // player is holding
  public class PlacerIndicator : MonoBehaviour {

    [SerializeField] private string indicatorID;

    public string IndicatorID {
      get { return indicatorID; }
    }

    void Start() {
      // hide in the beginning
      gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider collider) {
      // vibrate controllers only if layer is "InteractiveObject"
      if (collider.gameObject.layer != 9) return;

      GrabbableHands grabbableHands = collider.GetComponent<GrabbableHands>();

      OVRInput.Controller currentController;

      // decide which controller to vibrate
      if (grabbableHands) {
        if (grabbableHands.grabbedBy.gameObject.name == "HandLeft") {
          currentController = OVRInput.Controller.LTouch;
        } else {
          currentController = OVRInput.Controller.RTouch;
        }
        
      } else {
        currentController = OVRInput.Controller.Touch;
      }

      TouchHaptics.Instance.VibrateFor(0.5f, 0.2f, 0.2f, currentController);
    }

  }
}
