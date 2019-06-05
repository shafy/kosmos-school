using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // show controls in ingame menu
  public class IngameMenuControlsButton : TextureButton {

    [SerializeField] private IngameMenuController ingameMenuController;

    public override void Press () {
      base.Press();
      ingameMenuController.ShowControls();
    }
  }
}
