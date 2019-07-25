using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // we put this on every single waypoint
  public class RCSingleWaypoint : MonoBehaviour {

    private AudioSource audioSourceTrack;
    private AudioClip audioClipTrack;

    public AudioClip AudioClipTrack {
      get { return audioClipTrack; }
      set { audioClipTrack = value; }
    }

    void Start() {
      audioSourceTrack = gameObject.AddComponent<AudioSource>();
      audioSourceTrack.playOnAwake = false;
      audioSourceTrack.volume = 0.5f;
      audioSourceTrack.clip = audioClipTrack;
    }

    void OnTriggerEnter(Collider collider) {
      audioSourceTrack.Play();
    }
  }
}
