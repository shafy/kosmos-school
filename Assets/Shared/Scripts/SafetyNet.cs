using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Catches Items that have fallen through the Floor and respawns them
  public class SafetyNet : MonoBehaviour {

    [SerializeField] private Transform respawnParentTransform;

    void OnCollisionEnter(Collision collision) {
      // if gameObjects name is in respawnParentTransform, move GameObject back up there
      Transform childTransform = respawnParentTransform.Find(collision.transform.parent.gameObject.name);
      if (!childTransform) return;

      collision.transform.position = childTransform.position;
    }
  }
}
