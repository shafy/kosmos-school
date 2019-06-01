using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kosmos {
  // Ingame Menu Logic
  public class IngameMenuController : MonoBehaviour {

    [SerializeField] GameObject mainDisplay;
    [SerializeField] GameObject controlsDisplay;

    void Start() {
      ShowMain();
    }

    public void ShowMain() {
      mainDisplay.active = true;
      controlsDisplay.active = false;
    }

    public void ShowControls() {
      mainDisplay.active = false;
      controlsDisplay.active = true;
    }
  }
}
