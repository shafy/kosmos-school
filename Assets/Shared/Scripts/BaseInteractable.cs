using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos
{
  // the base class for items that the player can interact with
  // other classes such as Grabbable extend this class
  public class BaseInteractable : MonoBehaviour{

    private InteractiveItem m_InteractiveItem;
    //private ParticleSystem rayStars;
    private GameObject handAnchor;
    //private bool rayStarsEnabled;
    
    [System.NonSerialized()] public PlayerController PlayerController;

    public bool isOver {
      get;
      private set;
    }

    // public bool RayStarsEnabled {
    //   get { return rayStarsEnabled; }
    //   set { rayStarsEnabled = value; }
    // }

    public void Start() {
      m_InteractiveItem = GetComponent<InteractiveItem>();
      m_InteractiveItem.OnOver += HandleOver;
      m_InteractiveItem.OnOut += HandleOut;

      isOver = false;
      //rayStarsEnabled = true;

      PlayerController = GameObject.FindWithTag("OVRPlayerController").GetComponent<PlayerController>();
      handAnchor = PlayerController.HandAnchor();

      //rayStars = handAnchor.transform.Find("RayParticleSystem").gameObject.GetComponent<ParticleSystem>();
    }

    //Handle the Over event
    private void HandleOver(){
        isOver = true;
        //if (rayStarsEnabled) rayStars.Play();
    }

    private void HandleOut(){
        isOver = false;
        //if (rayStarsEnabled) rayStars.Stop();
    }

  }
}
