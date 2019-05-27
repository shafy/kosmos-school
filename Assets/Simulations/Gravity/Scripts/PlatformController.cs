using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Makes sure are moved to right place on platform if dropped on it
  public class PlatformController : MonoBehaviour {
    
    private bool isMoving;
    private bool ungrabbableCoroutineRunning;
    private float lerpValue;
    private float movementTime;
    private Vector3 startPos;
    private Vector3 endPos;
    private List<Transform> objectsList;
    private Transform currentObjectTransform;

    [SerializeField] private Transform[] SpotTransforms;

    public List<Transform> ObjectsList {
        get { return objectsList; }
        private set { objectsList = value;}
    }

    void Start() {
      objectsList = new List<Transform>();
      lerpValue = 0.0f;

      // move objects over movementTime seconds
      movementTime = 0.5f;

      isMoving = false;
      ungrabbableCoroutineRunning = false;
    }


    void Update() {
      if (!isMoving) return;

      if (!ungrabbableCoroutineRunning) {
        StartCoroutine(makeUngrabbable(movementTime, currentObjectTransform));
      }

      lerpValue += Time.deltaTime / movementTime;
      currentObjectTransform.position = Vector3.Lerp(startPos, endPos, lerpValue);

      // when arrived, stop moving
      if (currentObjectTransform.position == endPos) {
        isMoving = false;
        lerpValue = 0.0f;
      }
    }

    void OnTriggerEnter(Collider collider) {
      // if user drops an object, move it to the next free spot
      // first, check if the object has a GravityPhysics component
      GravityPhysics currentGP = collider.gameObject.GetComponent<GravityPhysics>();
      if (!currentGP) return;

      // do nothing if it's falling (i.e. IsActive == true)
      //if (currentGP.IsActive) return;

      // return if object is already in the list
      if (objectsList.Contains(collider.transform)) return;

      // move to next free pos
      moveToSpot(collider.transform, SpotTransforms[objectsList.Count].position);
    }

    void OnTriggerExit(Collider collider) {

      // remove from list
      if (!collider.gameObject.GetComponent<GravityPhysics>()) return;

      objectsList.Remove(collider.transform);
      repositionObjects();
    }

    // moves to next free spot
    private void moveToSpot(Transform transformToMove, Vector3 _endPos) {
      // if list is full, don't move
      if (SpotTransforms.Length == objectsList.Count) return;

      endPos = _endPos;
      // Add to List
      objectsList.Add(transformToMove);

      // asign current transform and pos
      startPos = transformToMove.position;
      currentObjectTransform = transformToMove;
      
      isMoving = true;
    }

    // respositions objects on the platform
    private void repositionObjects() {
      // loop thorugh objectsList
      int y = 0;
      for (int i = 0; i < objectsList.Count; i++) {
        // only move if not already at the same position
        if (objectsList[i].position.x != SpotTransforms[i].position.x) {
          objectsList[i].position = SpotTransforms[i].position;
          y++;
        }
      }
    }

    private IEnumerator makeUngrabbable(float waitTime, Transform currentTransform) {
      // make ungrabbable
      Grabbable currentGrabbable = currentObjectTransform.gameObject.GetComponent<Grabbable>();

      currentGrabbable.IsGrabbable = false;
      yield return new WaitForSeconds(waitTime);
      // make grabbable again
      currentGrabbable.IsGrabbable = true;
      ungrabbableCoroutineRunning = false;
    }
  }
}
