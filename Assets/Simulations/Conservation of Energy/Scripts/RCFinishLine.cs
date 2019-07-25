using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // finishes ride
  public class RCFinishLine : MonoBehaviour {

    private RollerCoasterBuilderController rollerCoasterBuilderController;

    void Start() {
      rollerCoasterBuilderController = GameObject.Find("RollerCoasterBuilder").GetComponent<RollerCoasterBuilderController>();
    }

    void OnTriggerEnter(Collider collider) {
      RollerCoasterCart rollerCoasterCart = collider.GetComponent<RollerCoasterCart>();

      if (!rollerCoasterCart) return;

      if (!rollerCoasterCart.IsRunning) return;

      rollerCoasterBuilderController.StartStopCart();
    }
  }
}
