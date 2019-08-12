using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos
{
  // this class makes an object grabbable
  public class Grabbable : BaseInteractable {

    private bool isGrabbed;
    private bool isPhantom;
    private Vector3 phantomPosition;

    [SerializeField] private bool isGrabbable = true;

    public bool IsGrabbable {
      get { return isGrabbable; }
      set { isGrabbable = value; }
    }

    public bool IsPhantom {
      get { return isPhantom; }
      set { isPhantom = value; }
    }

    public bool IsGrabbed {
      get { return isGrabbed; }
      set { isGrabbed = value; }
    }

    public Vector3 PhantomPosition {
      get { return phantomPosition; }
      set { phantomPosition = value; }
    }
    
    void Start() {
      base.Start();
    }

    void Update() {
      if (!isPhantom) {
        phantomPosition = transform.position;
      }
    }

    // this can be overriden in child
    public virtual void Grabbed() {
      isGrabbed = true;
    }

    // this can be overriden in child
    public virtual void Ungrabbed() {
      isGrabbed = false;
      isPhantom = false;
    }
  }
}
