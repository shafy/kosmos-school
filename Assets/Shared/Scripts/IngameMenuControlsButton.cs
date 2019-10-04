using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // show controls in ingame menu
  public class IngameMenuControlsButton : TextureButton {

    [SerializeField] private IngameMenuController ingameMenuController;

    [SerializeField] private Texture defaultTextureQuest;
    [SerializeField] private Texture hoverTextureQuest;

   void Awake() {
    base.Awake();
    // if Quest, assign new textures
    if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
      defaultTexture = defaultTextureQuest;
      hoverTexture = hoverTextureQuest;
    }
   }

    public override void Press () {
      base.Press();
      ingameMenuController.ShowControls();
    }
  }
}
