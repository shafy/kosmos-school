using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // light that can be turned on and off
  public class Light : MonoBehaviour {

    private Renderer renderer;

    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    void Start() {
      renderer = GetComponent<Renderer>();
    }

    public void TurnOn(bool enable) {
      if (enable) {
        renderer.material = onMaterial;
      } else {
        renderer.material = offMaterial;
      }
    }
  }
}
