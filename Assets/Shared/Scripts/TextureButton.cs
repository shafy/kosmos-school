using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos;

namespace Kosmos.Shared {
  // general button swith changing texture on hover
  public class TextureButton : Button {

    private bool wasOver;
    private Renderer renderer;

    [SerializeField] public Texture defaultTexture;
    [SerializeField] public Texture hoverTexture;

    void Start() {
      base.Start();

      renderer = GetComponent<Renderer>();
      renderer.material.mainTexture = defaultTexture;

      wasOver = false;
    }

    void Update() {

      // change to hover texture
      if (isOver && !wasOver) {
        renderer.material.mainTexture = hoverTexture;
        wasOver = true;

        TouchHaptics.Instance.VibrateFor(0.1f, 0.2f, 0.1f, OVRInput.Controller.LTouch);
      }


      // change back to default texture
      if (!isOver && wasOver) {
        renderer.material.mainTexture = defaultTexture;
        wasOver = false;
      }
    }
  }
}
