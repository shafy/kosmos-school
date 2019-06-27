using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // changes between items sizes
  public class ItemSizeButton : Button {

    [SerializeField] private enum Direction {Larger, Smaller};
    [SerializeField] private Direction direction;

    [SerializeField] private RollerCoasterBuilderController rollerCoasterBuilderController;


    public override void Press () {
      base.Press();

      if (direction == Direction.Larger) {
        rollerCoasterBuilderController.ChangeItemSize("larger");
      } else {
        rollerCoasterBuilderController.ChangeItemSize("smaller");
      }
      
    }
  }
}
