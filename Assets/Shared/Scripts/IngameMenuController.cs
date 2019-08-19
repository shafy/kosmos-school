using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kosmos {
  // Ingame Menu Logic
  public class IngameMenuController : MonoBehaviour {

    [SerializeField] private GameObject mainDisplay;
    [SerializeField] private GameObject controlsDisplay;
   
    [SerializeField] private Texture controlDescriptionQuest;
    [SerializeField] private Texture controlDescriptionGo;

    [SerializeField] private Renderer controlDescriptionRenderer;

    void Awake() {
      // put correct textures
      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
        controlDescriptionRenderer.material.mainTexture = controlDescriptionQuest;
      } else {
        controlDescriptionRenderer.material.mainTexture = controlDescriptionGo;
      }
    }

    void OnEnable() {
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
