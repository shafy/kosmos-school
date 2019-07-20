using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // changes between items
  public class ItemSwitchButton : Button {

    [SerializeField] private enum Direction {Next, Previous};
    [SerializeField] private Direction direction;

    [SerializeField] private RollerCoasterBuilderController rollerCoasterBuilderController;


    public override void Press () {
      base.Press();

      if (direction == Direction.Next) {
        rollerCoasterBuilderController.SwitchItem("next");
      } else {
        rollerCoasterBuilderController.SwitchItem("previous");
      }
      
    }
  }
}
