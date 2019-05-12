using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Generic Button
  public class Button : BaseInteractable {

    // override this in child class
    public virtual void Press() {}
    
  }
}
