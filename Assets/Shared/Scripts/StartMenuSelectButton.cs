using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // selects simulatin to start from start menu (welcome scren)
  public class StartMenuSelectButton : TextureButton {

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string sceneName;

    public override void Press () {
      base.Press();
      if (sceneLoader && sceneName != "") sceneLoader.LoadNewScene(sceneName);
    }
  }
}
