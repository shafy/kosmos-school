using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // places full sized roller coaster part in scene
  public class RCItemPlaceButton : Button {

    [SerializeField] private RollerCoasterBuilderController rollerCoasterBuilderController;


    public override void Press () {
      base.Press();

      rollerCoasterBuilderController.PlaceCurrentItem();
      
    }
  }
}
