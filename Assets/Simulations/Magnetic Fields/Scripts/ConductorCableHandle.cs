using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // logic for the conductor cable's handle
  public class ConductorCableHandle : MonoBehaviour {

    private bool isSnapped;
    private bool toSnap;
    private GrabbableHands grabbableHands;
    private Rigidbody rb;
    private PowerSourceConnector latestSnapPositionConnector;
    private Quaternion latestSnapRotation;

    [SerializeField] private ConductorMF currentConductorMF;

    public enum HandleSide {right, left};
    [SerializeField] private HandleSide currentHandleSide;

    public HandleSide CurrentHandleSide {
      get { return currentHandleSide; }
    }

    void Start() {
      isSnapped = false;
      toSnap = false;

      grabbableHands = GetComponent<GrabbableHands>();
      rb = GetComponent<Rigidbody>();
    }

    void Update() {

      // snap to last position
      if (!isSnapped && toSnap && !grabbableHands.isGrabbed) {
        transform.position = latestSnapPositionConnector.transform.position;
        // make sure it has correct rotation
        transform.rotation = latestSnapRotation * Quaternion.Euler(0, 90, 0);

        latestSnapPositionConnector.IsOccupied = true;
        latestSnapPositionConnector.AddConductor(currentConductorMF, currentHandleSide);
        
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
      PowerSourceConnector powerSourceConnector = collider.gameObject.GetComponent<PowerSourceConnector>();
      if (!powerSourceConnector) return;

      if (powerSourceConnector.IsOccupied) return;

      latestSnapPositionConnector = powerSourceConnector;
      latestSnapRotation = powerSourceConnector.PowerSourceRotation;
      toSnap = true;
    }

    void OnTriggerExit(Collider collider) {
      // check if it's a conductor
      PowerSourceConnector powerSourceConnector = collider.gameObject.GetComponent<PowerSourceConnector>();
      if (!powerSourceConnector) return;

      // reset
      toSnap = false;
      latestSnapPositionConnector.IsOccupied = false;
    }
  
    public ConductorMF GetConductorMF() {
      return currentConductorMF;
    }  
  
  }
}
