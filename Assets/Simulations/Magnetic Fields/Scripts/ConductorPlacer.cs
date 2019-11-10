using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // makes sure players can place the conductor on these hooks
  public class ConductorPlacer : MonoBehaviour {

    private AudioSource audioSource;
    private AudioClip audioPlacement;
    private bool isSnapped;
    private bool toSnap;
    private GrabbableHands grabbableHands;
    private Rigidbody rb;
    private ConductorStandHook latestSnapStandHook;
    private ConductorMF conductorMF;

    [SerializeField] private ConductorStandHook initialHook;
    [SerializeField] private MFController mfController;

    void Start() {
      conductorMF = GetComponent<ConductorMF>();

      isSnapped = false;
      toSnap = false;

      grabbableHands = GetComponent<GrabbableHands>();
      rb = GetComponent<Rigidbody>();

      // place at initial position if there's one referenced
      if (initialHook) {
        latestSnapStandHook = initialHook;
        toSnap = true;
      }

      audioSource = GetComponent<AudioSource>();
    }

    void Update() {

      if (!isSnapped && toSnap && !grabbableHands.isGrabbed) {
        // when user places it
        transform.position = latestSnapStandHook.ConductorSpotPosition;
        transform.rotation = Quaternion.identity;
        
        rb.isKinematic = true;

        isSnapped = true;
        toSnap = false;

        conductorMF.IsPlaced = true;
        mfController.ReDraw();

        if (audioSource) {
          audioSource.clip = audioPlacement;
          audioSource.Play();
        }

      }

      // when user removes it
      if (isSnapped && grabbableHands.isGrabbed) {
        isSnapped = false;
        toSnap = true;

        conductorMF.IsPlaced = false;
        mfController.ReDraw();
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