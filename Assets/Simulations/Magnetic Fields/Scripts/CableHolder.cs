using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos.Shared;

namespace Kosmos.MagneticFields {
  // default position for cables
  public class CableHolder : MonoBehaviour {

    private bool isOccupied;
    private Transform cableTransform;

    void Start() {
      isOccupied = false;
    }

    void Update() {
      if (!isOccupied) return;

      // make sure cable doesn't move
      cableTransform.position = transform.position;
      cableTransform.rotation = Quaternion.identity;
    }

    public void AddCable(Transform _cableTransform) {
      isOccupied = true;
      cableTransform = _cableTransform;
    }

    void OnTriggerExit(Collider collider) {
      if (collider.name != "Handle") return;

      isOccupied = false;
      cableTransform = null;
    }
  }
}
