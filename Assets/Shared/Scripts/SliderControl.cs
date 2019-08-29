using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // slider control that can be moved within a specified range
  // make sure to add and configure a Constrain Position component correctly
  [RequireComponent(typeof(Kosmos.Shared.ConstrainPosition))]
  public class SliderControl : MonoBehaviour {

    private float sliderValue;

    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    [Tooltip("How many meters it can move physically")]
    [SerializeField] private float movementRange;

    // enum
    public enum MovementAxis {X, Y, Z};
    [SerializeField] private MovementAxis movementAxis;


    void Start() {

    }

    void Update() {
      // make sure can't move outside of the defined range
      constrainMovementPos();

      sliderValue = (movementAxisPosValue() / movementRange) * (maxValue - minValue);      
    }

    private void constrainMovementPos() {
      // only do something if not in range
      if (movementAxisPosValue() >= 0f && movementAxisPosValue() <= movementRange) return;

      switch (movementAxis) {
        case MovementAxis.X:
           transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, 0, movementRange), transform.localPosition.y, transform.localPosition.z);
           break;
        case MovementAxis.Y:
           transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, 0, movementRange), transform.localPosition.z);
           break;
        case MovementAxis.Z:
           transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z, 0, movementRange));
           break;
      }
    }

    private float movementAxisPosValue() {
      switch (movementAxis) {
        case MovementAxis.X:
          return transform.localPosition.x;
        case MovementAxis.Y:
          return transform.localPosition.y;
        case MovementAxis.Z:
          return transform.localPosition.z;
        default:
          return transform.localPosition.x;
      }
    }
  }
}
