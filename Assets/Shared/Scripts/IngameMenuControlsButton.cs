using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // show controls in ingame menu
  public class IngameMenuControlsButton : Button {

    [SerializeField] private IngameMenuController ingameMenuController;

    public override void Press () {
      ingameMenuController.ShowControls();
    }
  }
}
