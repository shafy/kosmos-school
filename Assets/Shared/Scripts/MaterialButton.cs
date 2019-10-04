using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos;

namespace Kosmos.Shared {
  // general button with changing material on hover
  public class MaterialButton : Button {

    private bool wasOver;
    private Renderer renderer;

    private Material defaultMat;
    [SerializeField] public Material hoverMat;

    void Start() {
      base.Start();

      renderer = GetComponent<Renderer>();
      defaultMat = renderer.material;

      wasOver = false;
    }

    void Update() {

      // change to hover texture
      if (isOver && !wasOver) {
        renderer.material = hoverMat;
        wasOver = true;

        TouchHaptics.Instance.VibrateFor(0.1f, 0.2f, 0.1f, OVRInput.Controller.LTouch);
      }


      // change back to default texture
      if (!isOver && wasOver) {
        renderer.material = defaultMat;
        wasOver = false;
      }
    }
  }
}
