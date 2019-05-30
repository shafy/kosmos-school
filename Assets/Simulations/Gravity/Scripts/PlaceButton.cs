using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Places the object at the specified height
  public class PlaceButton : Button {

    private bool shouldMove;
    private float lerpValue;
    private float movementTime;
    private GravityPhysics currentGP;
    private List<Transform> platformItemsList;
    private List<Vector3> startPosList;
    private List<Vector3> endPosList;

    [SerializeField] private PlatformController platformController;

    void Start() {
      lerpValue = 0.0f;
      movementTime = 1.0f;
    }

    void Update() {
      if (!shouldMove) return;

      lerpValue += Time.deltaTime / movementTime;

      // loop through list and update position
      for (int i = 0; i < platformItemsList.Count; i++) {
        platformItemsList[i].position = Vector3.Lerp(startPosList[i], endPosList[i], lerpValue);
      }

      // when first item arrived, stop moving
      // this assumes that all items start from the same pos and move to same pos

      if (platformItemsList[0].position == endPosList[0]) {
        for (int i = 0; i < platformItemsList.Count; i++) {
          // deactivate read only mode
          platformItemsList[i].gameObject.GetComponent<GravityPhysics>().SetReadOnly(false);
        }
        shouldMove = false;
        platformController.UpdateIsPlaced(true);
        lerpValue = 0.0f;
      }
    }

    public override void Press () {
      // if objects are not place, place them, else, drop them
      if (platformController.IsPlaced) {
        for (int i = 0; i < platformItemsList.Count; i++) {
          platformItemsList[i].gameObject.GetComponent<GravityPhysics>().SetActive(true);
        }
        platformController.UpdateIsPlaced(false);
        return;
      }
      // if objects are not placed, place them

      // get current list from platformController (make sure to copy it)
      platformItemsList = new List<Transform>(platformController.ObjectsList);

       // if no objects, do nothing
      if (platformItemsList.Count == 0) return;

      // if placeHeight 0, don't place them
      if (platformController.PlaceHeight == 0.0f) return;

      // return if one of the list is readonly
      GravityPhysics currentGP = platformItemsList[0].gameObject.GetComponent<GravityPhysics>();
      if (currentGP.ReadOnly) return;

      // re-initiliaze lists
      startPosList = new List<Vector3>();
      endPosList = new List<Vector3>();

      // loop through list and set start and end positions
      for (int i = 0; i < platformItemsList.Count; i++) {
        startPosList.Add(platformItemsList[i].position);
        endPosList.Add(new Vector3(platformItemsList[i].position.x, platformController.PlaceHeight, platformItemsList[i].position.z));

        // make sure it's not active so that physics doesn't apply
        platformItemsList[i].gameObject.GetComponent<GravityPhysics>().SetActive(false);
      }

      shouldMove = true;
    }
  }
}
