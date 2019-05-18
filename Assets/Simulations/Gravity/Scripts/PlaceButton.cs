using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Places the object at the specified height
  public class PlaceButton : Button {

    private bool shouldMove;
    private float placeHeight;
    private float lerpValue;
    private float movementTime;
    private GravityPhysics currentGP;
    private Vector3 startPos;
    private Vector3 endPos;
    private Transform currentTransform;

    [SerializeField] private GameObject currentGO;

    void Start() {
      placeHeight = 50.0f;

      currentTransform = currentGO.transform;
      currentGP = currentGO.GetComponent<GravityPhysics>();

      lerpValue = 0.0f;
      movementTime = 3.0f;
      startPos = currentTransform.position;
      endPos = new Vector3(currentTransform.position.x, placeHeight, currentTransform.position.z);
    }

    void Update() {
      if (!shouldMove) return;

      lerpValue += Time.deltaTime / movementTime;
      currentTransform.position = Vector3.Lerp(startPos, endPos, movementTime);

      // when arrived, stop moving
      if (currentTransform.position == endPos) {
        // deactivate read only mode
        currentGP.SetReadOnly(false);
        shouldMove = false;
      }
    }

    public override void Press () {
      // set to read only mode
      if (currentGP.ReadOnly) return;
      // place the object at x meters
      shouldMove = true;
      startPos = currentTransform.position;
      // make sure it's not active so that physics doesn't apply
      currentGP.SetActive(false);
    }
  }
}
