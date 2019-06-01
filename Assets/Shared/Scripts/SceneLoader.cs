using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kosmos {
  // loads a new scene
  public class SceneLoader : MonoBehaviour {

    public void LoadNewScene(string sceneName) {
      StartCoroutine(LoadAsyncScene(sceneName));
    }

    private IEnumerator LoadAsyncScene(string sceneName) {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

      while (!asyncLoad.isDone) {
        yield return null;
      }
    }
  }
}
