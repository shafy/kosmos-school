using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // remove full sized roller coaster part in scene
  public class RCItemRemoveButton : Button {

    [SerializeField] private RollerCoasterBuilderController rollerCoasterBuilderController;

    public override void Press () {
      base.Press();

      rollerCoasterBuilderController.RemoveLastItem();
      
    }
  }
}
