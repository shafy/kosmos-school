using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // finishes ride
  public class RCFinishLine : MonoBehaviour {

    void OnTriggerEnter(Collider collider) {
      RollerCoasterCart rollerCoasterCart = collider.GetComponent<RollerCoasterCart>();

      if (!rollerCoasterCart) return;

      rollerCoasterCart.FinishRide();
    }
  }
}
