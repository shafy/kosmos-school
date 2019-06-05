using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // mixes and meshes different audiosources for ambience background sound
  public class AmbianceSoundMixer : MonoBehaviour {

    private bool play1;
    private bool play2;
    private float timer1;
    private float randomWaitTime1;
    private float timer2;
    private float randomWaitTime2;
    private float clipLength1;
    private float clipLength2;

    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;

    void Start() {
      if (audioSource1) clipLength1 = audioSource1.clip.length;
      if (audioSource1) clipLength2 = audioSource2.clip.length;

      Debug.Log("clipLength1 " + clipLength1);

      play1 = false;
      timer1 = 0.0f;
      randomWaitTime1 = randomWaitTime(0.0f, 10.0f);

      play2 = false;
      timer2 = 0.0f;
      randomWaitTime2 = randomWaitTime(0.0f, 20.0f);
    }

    void Update() {
      // we can generalize this later on with an array and for loop
      // start playing clip 1
      if (play1) {
        audioSource1.Play();
        play1 = false;
      }

      if (play2) {
        audioSource2.Play();
        play2 = false;
      }

      if (timer1 > randomWaitTime1) {
        play1 = true;
        timer1 = 0.0f;
        // set next timer to know when to start playing
        randomWaitTime1 = randomWaitTime(clipLength1, clipLength1 + 60.0f);
      }

      if (timer2 > randomWaitTime2) {
        play2 = true;
        timer2 = 0.0f;
        // set next timer to know when to start playing
        randomWaitTime2 = randomWaitTime(clipLength2, clipLength2 + 100.0f);
      }

      timer1 += Time.deltaTime;
      timer2 += Time.deltaTime;
     
    }

    private float randomWaitTime(float length, float randomizer) {
      return Random.Range(length, clipLength1 + randomizer);
    }
  }
}