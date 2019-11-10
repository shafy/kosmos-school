using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // slider control that can be moved within a specified range
  // make sure to add and configure a Constrain Position component correctly
  [RequireComponent(typeof(Kosmos.Shared.ConstrainPosition))]
  public class SliderControl : MonoBehaviour {

    private AudioSource audioSource;
    private float sliderValue;
    private float externalSliderValue;
    private float prevSliderValue;
    private GrabbableHands grabbableHands;

    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField] private float initialValue;

    [Tooltip("How many meters it can move physically")]
    [SerializeField] private float movementRange;

    // enum
    public enum MovementAxis {X, Y, Z};
    [SerializeField] private MovementAxis movementAxis;

    public float SliderValue {
      get { return externalSliderValue; }
    }

    public bool IsGrabbed {
      get { return grabbableHands.isGrabbed; }
    }

    void Start() {
      grabbableHands = GetComponent<GrabbableHands>();

      sliderValue = initialValue;
      prevSliderValue = initialValue;

      if (initialValue != 0) {
        float initialPos = (initialValue / movementRange) * (maxValue - minValue);
        setInitialPosition(initialPos);
      }

      audioSource = GetComponent<AudioSource>();
    }

    void Update() {
      sliderValue = (movementAxisPosValue() / movementRange) * (maxValue - minValue);

      if (sliderValue == prevSliderValue) return;

      // make sure can't move outside of the defined range
      constrainMovementPos();

      externalSliderValue = Mathf.Clamp(sliderValue, minValue, maxValue);

      prevSliderValue = sliderValue;

      // play audio if it's not playing
      if (audioSource && !audioSource.isPlaying) {
        audioSource.Play();
      }

      // vibrate while moving
      GrabbableHands grabbableHands = GetComponent<Collider>().GetComponent<GrabbableHands>();

      OVRInput.Controller currentController;

      // decide which controller to vibrate
      if (grabbableHands) {
        if (grabbableHands.grabbedBy.gameObject.name == "HandLeft") {
          currentController = OVRInput.Controller.LTouch;
        } else {
          currentController = OVRInput.Controller.RTouch;
        }
      } else {
        currentController = OVRInput.Controller.Touch;
      }

      TouchHaptics.Instance.VibrateFor(0.5f, 0.2f, 0.2f, currentController);
    }

    private void constrainMovementPos() {
      // only do something if not in range
      if (movementRange > 0 && movementAxisPosValue() >= 0f && movementAxisPosValue() <= movementRange) return;

      if (movementRange < 0 && movementAxisPosValue() <= 0f && movementAxisPosValue() >= movementRange) return;

      transform.localPosition = Constrain();
    }

    private void setInitialPosition(float initialPos) {
      Vector3 newPos;
      switch (movementAxis) {
        case MovementAxis.X:
          newPos = new Vector3(initialPos, transform.localPosition.y, transform.localPosition.z);
          break;
        case MovementAxis.Y:
          newPos = new Vector3(transform.localPosition.x, initialPos, transform.localPosition.z);
          break;
        case MovementAxis.Z:
          newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, initialPos);
          break;
        default:
          newPos = new Vector3(initialPos, transform.localPosition.y, transform.localPosition.z);
          break;
      }
      transform.localPosition = newPos;
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

    public Vector3 Constrain() {
      Vector3 newLocalPos = Vector3.zero;

      switch (movementAxis) {
        case MovementAxis.X:
          if (movementRange > 0) {
            newLocalPos = new Vector3(Mathf.Clamp(transform.localPosition.x, 0, movementRange), transform.localPosition.y, transform.localPosition.z);
          } else {
            newLocalPos = new Vector3(Mathf.Clamp(transform.localPosition.x, movementRange, 0), transform.localPosition.y, transform.localPosition.z);
          }
          break;
        case MovementAxis.Y:
          if (movementRange > 0) {
            newLocalPos = new Vector3(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, 0, movementRange), transform.localPosition.z);
          } else {
            newLocalPos = new Vector3(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, movementRange, 0), transform.localPosition.z);
          }
          
           break;
        case MovementAxis.Z:
          if (movementRange > 0) {
            newLocalPos = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z, 0, movementRange));
          } else {
            newLocalPos = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z, movementRange, 0));
          }
          break;
      }

      return newLocalPos;
    }
  }
}
