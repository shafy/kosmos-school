using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Increases or decreases the height of the objects
  public class HeightButton : Button {

    private float incrementStep;

    [SerializeField] private enum Direction {Increase, Decrease};
    [SerializeField] private Direction direction;

    [SerializeField] private PlatformController platformController;


    void Start() {
      incrementStep = 10.0f;
    }

    public override void Press () {
      base.Press();
      
      float incrementalValue = incrementStep;

      if (direction == Direction.Decrease) {
        incrementalValue *= -1;
      }

      // increment value
      platformController.IncrementHeightValue(incrementalValue);
    }
  }
}
