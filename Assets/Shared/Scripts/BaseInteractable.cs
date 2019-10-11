using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos
{
  // the base class for items that the player can interact with
  // other classes such as Grabbable extend this class
  public class BaseInteractable : MonoBehaviour{

    private InteractiveItem m_InteractiveItem;

    public bool isOver {
      get;
      private set;
    }

    public void Start() {
      m_InteractiveItem = GetComponent<InteractiveItem>();

      if (m_InteractiveItem) {
        m_InteractiveItem.OnOver += HandleOver;
        m_InteractiveItem.OnOut += HandleOut;
      }
     

      isOver = false;
    }

    //Handle the Over event
    private void HandleOver(){
        isOver = true;
    }

    private void HandleOut(){
        isOver = false;
    }

  }
}
