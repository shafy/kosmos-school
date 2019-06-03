using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // closes ingame menu
  public class IngameMenuBackButton : TextureButton {

    [SerializeField] private GameController gameController;

    public override void Press () {
      gameController.ToggleIngameMenu();
    }
  }
}
