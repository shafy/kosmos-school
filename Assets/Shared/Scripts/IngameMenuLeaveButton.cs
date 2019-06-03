using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // leaves from simulation back to start menu
  public class IngameMenuLeaveButton : TextureButton {

    [SerializeField] private SceneLoader sceneLoader;

    public override void Press () {
      sceneLoader.LoadNewScene("WelcomeScreen");
    }
  }
}
