using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // makes sure players can place the conductor on these hooks
  public class ConductorStandHook : MonoBehaviour {

    private Vector3 conductorSpotPosition;

    [SerializeField] private Transform conductorSpotTransform;


    public Vector3 ConductorSpotPosition {
      get { return conductorSpotPosition; }
    }

    void Start() {
      conductorSpotPosition = conductorSpotTransform.position;
    }

  }
}