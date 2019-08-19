using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // leaves from simulation back to start menu
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
      }


      // change back to default texture
      if (!isOver && wasOver) {
        renderer.material.mainTexture = defaultTexture;
        wasOver = false;
      }
    }
  }
}
