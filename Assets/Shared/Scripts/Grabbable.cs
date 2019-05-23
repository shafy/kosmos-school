using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos
{
  // this class makes an object grabbable
  public class Grabbable : BaseInteractable {

    private bool isGrabbed;
    private float DistanceToObj;
    private Rigidbody rb;

    [SerializeField] private bool isGrabbable = true;

    public bool IsGrabbable {
      get { return isGrabbable; }
      set { isGrabbable = value; }
    }
    
    void Start() {
      base.Start();
      rb = GetComponent<Rigidbody>();
    }

    protected void gravityActive(bool active) {
      if (rb) {
        rb.useGravity = active;
      }
    }

    protected void kinematicActive(bool active) {
      if (rb) {
        rb.isKinematic = active;
      }
    }

    // this can be overriden in child
    public virtual void Grabbed() {
      isGrabbed = true;
    }

    // this can be overriden in child
    public virtual void Ungrabbed() {
      isGrabbed = false;
      // reset distance to obj
      DistanceToObj = 0;
    }
  }
}
