using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos
{
  // this class makes an object grabbable
  public class Grabbable : BaseInteractable {

    private bool isGrabbed;

    [SerializeField] private bool isGrabbable = true;

    public bool IsGrabbable {
      get { return isGrabbable; }
      set { isGrabbable = value; }
    }
    
    void Start() {
      base.Start();
    }

    // this can be overriden in child
    public virtual void Grabbed() {
      isGrabbed = true;
    }

    // this can be overriden in child
    public virtual void Ungrabbed() {
      isGrabbed = false;
    }
  }
}
