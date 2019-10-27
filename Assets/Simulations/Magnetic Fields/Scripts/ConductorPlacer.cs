using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // makes sure players can place the conductor on these hooks
  public class ConductorPlacer : MonoBehaviour {

    private bool isSnapped;
    private bool toSnap;
    private GrabbableHands grabbableHands;
    private Rigidbody rb;
    private ConductorStandHook latestSnapStandHook;

    [SerializeField] private ConductorStandHook initialHook; 

    void Start() {
      isSnapped = false;
      toSnap = false;

      grabbableHands = GetComponent<GrabbableHands>();
      rb = GetComponent<Rigidbody>();

      // place at initial position if there's one referenced
      if (initialHook) {
        latestSnapStandHook = initialHook;
        toSnap = true;
      }
    }

    void Update() {

      if (!isSnapped && toSnap && !grabbableHands.isGrabbed) {
        transform.position = latestSnapStandHook.ConductorSpotPosition;
        transform.rotation = Quaternion.identity;
        
        rb.isKinematic = true;

        isSnapped = true;
        toSnap = false;
      }

      // this happens when it's snapped, the player grabs it but doesn't move it enough
      // to exit the trigger collider. so it snaps right back to the last spot when let go.
      if (isSnapped && grabbableHands.isGrabbed) {
        isSnapped = false;
        toSnap = true;
      }
    }

    void OnTriggerEnter(Collider collider) {
      ConductorStandHook conductorStandHook = collider.gameObject.GetComponent<ConductorStandHook>();
      if (!conductorStandHook) return;

      latestSnapStandHook = conductorStandHook;
      toSnap = true;
    }

    void OnTriggerExit(Collider collider) {
      // check if it's a conductor
      ConductorStandHook conductorStandHook = collider.gameObject.GetComponent<ConductorStandHook>();
      if (!conductorStandHook) return;

      if (latestSnapStandHook.gameObject.name != collider.name) return;

      // reset
      toSnap = false;
    }
  }
}