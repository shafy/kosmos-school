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
    private Vector3 latestSnapPosition;

    void Start() {
      isSnapped = false;
      toSnap = false;
      latestSnapPosition = Vector3.zero;

      grabbableHands = GetComponent<GrabbableHands>();
      rb = GetComponent<Rigidbody>();
    }

    void Update() {
      //if (latestSnapPosition == Vector3.zero) return;
      // if (!toSnap) return;
      // if (grabbableHands.isGrabbed) return;

      if (!isSnapped && toSnap && !grabbableHands.isGrabbed) {
        transform.position = latestSnapPosition;
        transform.rotation = Quaternion.identity;
        
        //latestSnapPosition = Vector3.zero;
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

      // if not grabbing and there is a latest snap position, move conductor to that spot

      // transform.position = latestSnapPosition;
      // transform.rotation = Quaternion.identity;
      
      // latestSnapPosition = Vector3.zero;
      // rb.isKinematic = true;

      // isSnapped = true;
    }

    void OnTriggerEnter(Collider collider) {
      ConductorStandHook conductorStandHook = collider.gameObject.GetComponent<ConductorStandHook>();
      if (!conductorStandHook) return;

      latestSnapPosition = conductorStandHook.ConductorSpotPosition;
      toSnap = true;
    }

    void OnTriggerExit(Collider collider) {
      // check if it's a conductor
      ConductorStandHook conductorStandHook = collider.gameObject.GetComponent<ConductorStandHook>();
      if (!conductorStandHook) return;

      // reset
      //latestSnapPosition = Vector3.zero;
      toSnap = false;
    }

    // void OnCollisionEnter(Collision collision) {
    //   //Debug.Log("yooooo");
    //   // check if it's a conductor
    //   ConductorStandHook conductorStandHook = collision.gameObject.GetComponent<ConductorStandHook>();
    //   if (!conductorStandHook) return;

    //   Debug.Log("collision enter");
    //   rb.isKinematic = true;
    //   // save conductor place spot so we can place it there once user let's go
    //   latestSnapPosition = conductorStandHook.ConductorSpotPosition;
    // }

    // void OnCollisionExit(Collision collision) {
    //   // check if it's a conductor
    //   ConductorStandHook conductorStandHook = collision.gameObject.GetComponent<ConductorStandHook>();
    //   if (!conductorStandHook) return;

    //   Debug.Log("collision exit");

    //   // reset
    //   rb.isKinematic = false;
    //   latestSnapPosition = Vector3.zero;
    // }
  }
}