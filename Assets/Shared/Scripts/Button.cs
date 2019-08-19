using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Generic Button
  public class Button : BaseInteractable {

    private AudioSource audioSource;
    private bool isVibrating;
    private OVRInput.Controller currentController;

    [SerializeField] private AudioClip audioClip;

    public void Awake() {
      // we either use the AudioSource component on the button
      // or the AudioClip with the AudioSource Component from OVRPlayerController
      audioSource = GetComponent<AudioSource>();

      if (!audioSource && audioClip) {
        audioSource = GameObject.FindWithTag("UIAudioSource").GetComponent<AudioSource>();
      }

      isVibrating = false;
    }

    // ** QUEST ONLY **
    private IEnumerator stopVibrationCoroutine(OVRInput.Controller controller) {
      yield return new WaitForSeconds(0.25f);
      OVRInput.SetControllerVibration (0f, 0f, controller);
      isVibrating = false;
     } 

    // ** QUEST ONLY **
    public virtual void OnTriggerEnter(Collider collider) {
      if (collider.CompareTag("RightHandPointerCollider") || collider.CompareTag("LeftHandPointerCollider")) {

        if (collider.CompareTag("RightHandPointerCollider")) {
          currentController = OVRInput.Controller.RTouch;
        } else {
          currentController = OVRInput.Controller.LTouch;
        }
        
        if (!isVibrating) OVRInput.SetControllerVibration(0.2f, 0.2f, currentController);
        isVibrating = true;
        StartCoroutine(stopVibrationCoroutine(currentController));
        Press();
      }
      return;
    }
   
    // override this in child class
    // ** GO ONLY **
    public virtual void Press() {
      if (audioClip && audioSource) {
        audioSource.clip = audioClip;
      }
      
      if (audioSource) {
        audioSource.Play();
      }
    }
  }
}
