using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // goes back to main display of ingame menu
  public class IngameMenuControlsBackButton : Button {

    [SerializeField] private IngameMenuController ingameMenuController;

    public override void Press () {
      ingameMenuController.ShowMain();
    }
  }
}
