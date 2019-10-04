using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // Takes care of haptics for Quest
  public class TouchHaptics : MonoBehaviour {

    private static TouchHaptics _instance;

    public static TouchHaptics Instance {
      get { return _instance; }
    }

    private bool isVibrating;

    void Awake() {
      if (_instance != null && _instance != this) {
        Destroy(this.gameObject);
      } else {
        _instance = this;
      }
      
      isVibrating = false;
    }

    private IEnumerator stopVibrationCoroutine(float vibrationDuration, OVRInput.Controller controller) {
      isVibrating = true;
      yield return new WaitForSeconds(vibrationDuration);
      OVRInput.SetControllerVibration (0f, 0f, controller);
      isVibrating = false;
    } 

    public void VibrateFor(float vibrationDuration, float vibrationFrequence, float vibrationAmplitude, OVRInput.Controller controller) {

      if (isVibrating) return;

      if (UnityEngine.XR.XRDevice.model != "Oculus Quest") return;

      OVRInput.SetControllerVibration(vibrationFrequence, vibrationAmplitude, controller);
      StartCoroutine(stopVibrationCoroutine(vibrationDuration, controller));
    }
  }
}