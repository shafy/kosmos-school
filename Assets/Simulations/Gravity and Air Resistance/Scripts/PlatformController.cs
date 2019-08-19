using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kosmos {
  // Makes sure are moved to right place on platform if dropped on it
  public class PlatformController : MonoBehaviour {
    
    private bool isMoving;
    private bool ungrabbableCoroutineRunning;
    private bool isPlaced;
    private float lerpValue;
    private float movementTime;
    private float placeHeight;
    private int dropCompleteCounter;
    private List<Transform> objectsList;
    private Transform currentObjectTransform;
    private Vector3 startPos;
    private Vector3 endPos;

    [SerializeField] private GraphCreator graphCreator;
    [SerializeField] private Transform[] SpotTransforms;
    [SerializeField] private TextMeshPro heightLabelTMP;
    [SerializeField] private TextMeshPro heightValueTMP;
    [SerializeField] private TextMeshPro heightInstructionsTMP;
    [SerializeField] private TextMeshPro dropInstructionsTMP;
    [SerializeField] private TextMeshPro placeInstructionsTMP;

    public bool IsPlaced {
      get { return isPlaced; }
      private set { isPlaced = value;}
    }

    public float PlaceHeight {
      get { return placeHeight; }
      private set { placeHeight = value;}
    }

    public List<Transform> ObjectsList {
      get { return objectsList; }
      private set { objectsList = value;}
    }

    public GraphCreator GraphCreator {
      get { return graphCreator; }
      private set { graphCreator = value;}
    }

    void Start() {
      objectsList = new List<Transform>();
      lerpValue = 0.0f;
      isPlaced = false;

      // move objects over movementTime seconds
      movementTime = 0.5f;

      isMoving = false;
      ungrabbableCoroutineRunning = false;

      placeHeight = 10.0f;
      heightValueTMP.SetText("{0:0} m", placeHeight);

      // deactivate texts
      heightLabelTMP.gameObject.active = false;
      heightValueTMP.gameObject.active = false;
      heightInstructionsTMP.gameObject.active = false;
      dropInstructionsTMP.gameObject.active = false;

      //activate text
      placeInstructionsTMP.gameObject.active = true;

      createEmptyGraphs();
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

      // can't place new objects when objects are placed
      if (isPlaced) return;

      // check if the object has a GravityPhysics component
      GravityPhysics currentGP = collider.gameObject.GetComponent<GravityPhysics>();
      if (!currentGP) return;

      // dont' add to list if it's already full
      if (SpotTransforms.Length == objectsList.Count) return;

      // return if object is already in the list
      if (objectsList.Contains(collider.transform)) return;

      // activate texts if it's first objects to be added
      if (objectsList.Count == 0) {
        heightLabelTMP.gameObject.active = true;
        heightValueTMP.gameObject.active = true;
        heightInstructionsTMP.gameObject.active = true;
        placeInstructionsTMP.gameObject.active = false;
      }

      // Add to List
      objectsList.Add(collider.transform);

      // move to next free pos
      moveToSpot(collider.transform, SpotTransforms[objectsList.Count - 1].position);

      // make sure it's kinematic
      collider.GetComponent<Rigidbody>().isKinematic = true;
      collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void OnTriggerExit(Collider collider) {

      // remove from list
      if (!collider.gameObject.GetComponent<GravityPhysics>()) return;

      objectsList.Remove(collider.transform);
      repositionObjects();

      // deactivate texts if it's first objects to be added
      if (objectsList.Count == 0) {
        heightLabelTMP.gameObject.active = false;
        heightValueTMP.gameObject.active = false;
        heightInstructionsTMP.gameObject.active = false;
        placeInstructionsTMP.gameObject.active = true;
      }

      // make sure it's not kinematic (for Quest only)
      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
        collider.GetComponent<Rigidbody>().isKinematic = true;
      }
    }

    // moves to next free spot
    private void moveToSpot(Transform transformToMove, Vector3 _endPos) {
      endPos = _endPos;

      // assign current transform and pos
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

    // creates empty graphs that are later used to be filled with data
    private void createEmptyGraphs() {
      GraphableDescription speedGraph = new GraphableDescription("Speed", "Speed", "Time [s]", "Speed [m/s]");
      GraphableDescription accelerationGraph = new GraphableDescription("Acceleration", "Acceleration", "Time [s]", "Acceleration [m/s^2]");
      GraphableDescription distanceGraph = new GraphableDescription("Distance", "Distance", "Time [s]", "Distance [m]");
      GraphableDescription fResGraph = new GraphableDescription("F(res)", "F(res)", "Time [s]", "F(res) [N]");

      graphCreator.CreateEmptyGraph(speedGraph);
      graphCreator.CreateEmptyGraph(accelerationGraph);
      graphCreator.CreateEmptyGraph(distanceGraph);
      graphCreator.CreateEmptyGraph(fResGraph);
    }

    public void IncrementHeightValue(float incrementalValue) {
      float newValue = placeHeight + incrementalValue;
        
      // lower limit
      if (newValue < 10.0f) return;

      // upper limit
      if (newValue > 100.0f) return;

      placeHeight = newValue;

      // update TextMeshPro text
      heightValueTMP.SetText("{0:0} m", placeHeight);
    }

    public void UpdateIsPlaced(bool value) {
      isPlaced = value;

      // activate or deactivate text
      if (value) {
        dropInstructionsTMP.gameObject.active = true;
        heightInstructionsTMP.gameObject.active = false;

        // clear graph also
        graphCreator.ClearGraphs();
        createEmptyGraphs();
        
      } else {
        dropInstructionsTMP.gameObject.active = false;
        heightInstructionsTMP.gameObject.active = true;
      }
    }

    public void DropComplete(bool value) {
      if (!value) return;

      dropCompleteCounter += 1;

      // if all objects have dropped, graph line chart
      if (dropCompleteCounter == objectsList.Count) {
        graphCreator.CreateGraph();
        dropCompleteCounter = 0;
      }
    }
  }
}
