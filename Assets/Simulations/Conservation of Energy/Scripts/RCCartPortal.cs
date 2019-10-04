using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using mixpanel;

namespace Kosmos {
  // transports user to cart
  public class RCCartPortal : MonoBehaviour {

    private RollerCoasterCart rollerCoasterCart;

    void OnTriggerEnter(Collider collider) {

    if (collider.tag != "OVRPlayerController") return;

    // parent to cart
    rollerCoasterCart.SyncPlayerController(true);

    // track
    var props = new Value();
    props["Scene Name"] = SceneManager.GetActiveScene().name;
    Mixpanel.Track("Rode Rollercoaster", props);

    // start cart
    StartCoroutine(StartCartCoroutine());
    }

    private IEnumerator StartCartCoroutine() {
      yield return new WaitForSeconds(4.0f);
      rollerCoasterCart.StartStop();
    }

    public void AddCartReference(RollerCoasterCart _rollerCoasterCart) {
      rollerCoasterCart = _rollerCoasterCart;
    }
  } 
}
