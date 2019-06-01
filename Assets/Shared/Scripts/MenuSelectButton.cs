using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // selects a simulation to entet from welcomescreen
  public class MenuSelectButton : Button {

    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] string sceneName;

    public override void Press () {
      sceneLoader.LoadNewScene(sceneName);
    }
  }
}
