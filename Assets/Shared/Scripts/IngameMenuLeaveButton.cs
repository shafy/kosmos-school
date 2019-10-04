using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // leaves from simulation back to start menu
  public class IngameMenuLeaveButton : TextureButton {

    [SerializeField] private SceneLoader sceneLoader;

    public override void Press () {
      base.Press();
      sceneLoader.LoadNewScene("WelcomeScreen");
    }
  }
}
