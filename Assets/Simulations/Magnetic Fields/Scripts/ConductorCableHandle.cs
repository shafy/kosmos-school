using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // logic for the conductor cable's handle
  public class ConductorCableHandle : MonoBehaviour {

    private AudioSource audioSource;
    private bool isSnapped;
    private bool toSnap;
    private GrabbableHands grabbableHands;
    private Rigidbody rb;
    private PowerSourceConnector latestSnapPositionConnector;
    private Quaternion latestSnapRotation;

    [SerializeField] private ConductorMF currentConductorMF;
    [SerializeField] private PowerSourceConnector initialPowerConnector;
    [SerializeField] private CableHolder cableHolder;

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

      // place at initial position if there's one referenced
      if (initialPowerConnector) {
        latestSnapPositionConnector = initialPowerConnector;
        latestSnapRotation = latestSnapPositionConnector.PowerSourceRotation;
        toSnap = true;
      }

      audioSource = GetComponent<AudioSource>();
    }

    void Update() {


      // don't change position and rotation if it's snapped
      if (isSnapped && !grabbableHands.isGrabbed) {
        transform.position = latestSnapPositionConnector.transform.position;
        transform.rotation = latestSnapPositionConnector.PowerSourceRotation * Quaternion.Euler(0, 90, 0);
      }

      // snap to last position
      if (!isSnapped && toSnap && !grabbableHands.isGrabbed) {

        latestSnapPositionConnector.IsOccupied = true;
        latestSnapPositionConnector.AddConductor(currentConductorMF, currentHandleSide);
        
        rb.isKinematic = true;

        isSnapped = true;
        toSnap = false;

        if (audioSource) audioSource.Play();
      }

      // this happens when player removes a handle
      if (isSnapped && grabbableHands.isGrabbed) {
        isSnapped = false;
        toSnap = true;

        latestSnapPositionConnector.RemoveConductor();
        latestSnapPositionConnector.IsOccupied = false;
      }
    }

    void OnTriggerEnter(Collider collider) {
      PowerSourceConnector powerSourceConnector = collider.gameObject.GetComponent<PowerSourceConnector>();
      if (!powerSourceConnector) return;

      //if (powerSourceConnector.IsOccupied) return;
      if (!powerSourceConnector.IsAddable(currentConductorMF, currentHandleSide)) return;

      latestSnapPositionConnector = powerSourceConnector;
      latestSnapRotation = powerSourceConnector.PowerSourceRotation;
      toSnap = true;
    }

    void OnTriggerExit(Collider collider) {
      // check if it's a conductor
      PowerSourceConnector powerSourceConnector = collider.gameObject.GetComponent<PowerSourceConnector>();
      if (!powerSourceConnector) return;

      // only keep executing if the exit is from the same collider as the most recent entry
      // otherwise, overlapping enters and exits can have undesirable behavior
      if (latestSnapPositionConnector.gameObject.name != collider.name) return;

      // reset
      toSnap = false;
    }

    void OnCollisionEnter(Collision collision) {
      if (!collision.gameObject.CompareTag("TerrainFloor") || grabbableHands.isGrabbed) return;

      // if collided with terrain, return to cable holder floating pos
      cableHolder.AddCable(transform);
      rb.isKinematic = true;
    }
  
    public ConductorMF GetConductorMF() {
      return currentConductorMF;
    }  
  
  }
}
