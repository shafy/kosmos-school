using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Generic Button
  public class Button : BaseInteractable {

    private AudioSource audioSource;

    [SerializeField] private AudioClip audioClip;

    void Awake() {
      // we either use the AudioSource component on the button
      // or the AudioClip with the AudioSource Component from OVRPlayerController
      audioSource = GetComponent<AudioSource>();
      if (!audioSource && audioClip) {
        audioSource = GameObject.FindWithTag("UIAudioSource").GetComponent<AudioSource>();
      }
    }
   
    // override this in child class
    public virtual void Press() {
      if (audioClip) {
        audioSource.clip = audioClip;
      }
      
      if (audioSource) {
        audioSource.Play();
      }
      
    }
    
  }
}
