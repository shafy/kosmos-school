using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // handles roller coaster cart logic
  public class RollerCoasterCart : KosmosPhysics {

    private bool isRunning;
    private bool isMoving;
    private bool isFirstWaypoint;
    private float currentSegmentLength;
    private float currentSpeed;
    private float timeCounter;
    private float distCovered;
    private float fracSegment;
    private float massCart;
    private float accelerationGravity;
    private float eTot;
    private float ePot;
    private float eKin;
    private float currentEndVelocity;
    private float currentStartVelocity;
    private float currentAcceleration;
    private float pushAcceleration;
    private int waypointIndex;
    private int waypointSystemsIndex;
    private Vector3 prevWaypointPos;
    private Vector3 nextWaypointPos;
    private Vector3 currentUnitVectorInter;

    [SerializeField] private RollerCoasterController rollerCoasterController;
    [SerializeField] private Transform wheelsFront;
    [SerializeField] private Transform wheelsBack;
    [SerializeField] private Transform sensorTop;
    [SerializeField] private Transform sensorBottom;

    void Awake() {
      // mass in kg
      massCart = 700f;
      accelerationGravity = 9.81f;
      //pushAcceleration = 1.0f;
      //isFirstWaypoint = true;
    }

    void Update() {

      if (!isRunning) return; 

      if (!isMoving) return;

      // if (isFirstWaypoint) {
      //   // if it's the first waypoint, we have to give the cart a push so to speak
      //   currentAcceleration = pushAcceleration;
      //   isFirstWaypoint = false;
      // }

      timeCounter += Time.deltaTime;
     
      // Debug.Log("currentStartVelocity " + currentStartVelocity);
      // Debug.Log("currentAcceleration " + currentAcceleration);

      // calculate speed for current time baed on start velocity and acceleration
      currentSpeed = currentStartVelocity + currentAcceleration * timeCounter;

      //Debug.Log("currentSpeed " + currentSpeed);

      // calculate what fraction of the segment we've already covered in order to lerp
      distCovered = timeCounter * currentSpeed;
      fracSegment = distCovered / currentSegmentLength;

      transform.position = Vector3.Lerp(prevWaypointPos, nextWaypointPos, fracSegment);

      // update rotation
      transform.rotation = Quaternion.FromToRotation(transform.forward, currentUnitVectorInter) * transform.rotation;

      // handle z rotation
      Vector3 WaypointVec = rollerCoasterController.WaypointSystemsList[waypointSystemsIndex].WaypointList[waypointIndex].WaypointVec;
      Quaternion tiltRotation = Quaternion.FromToRotation(transform.right, WaypointVec) * transform.rotation;

      // apply z rotation
      float currentZRotation = tiltRotation.eulerAngles.z;
      transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, currentZRotation);

      rotateWheels(currentSpeed * 50);
    }

    void OnTriggerEnter(Collider collider) {
      // check name to see if it's a waypoint
      if (!collider.CompareTag("Waypoint")) return;

      toNextWaypoint();
    }

    // velocity at the end of the current segment
    private float calculateEndVelocity() {
      return Mathf.Sqrt(eKin * 2 / massCart);
    }

    private float calculateAcceleration() {
      return
        (Mathf.Pow(currentEndVelocity, 2.0f) - Mathf.Pow(currentStartVelocity, 2.0f)) /
        (currentSegmentLength * 2);
    }

    // sets cart to position of first waypoint
    private void setStartPos() {
      transform.position = rollerCoasterController.WaypointSystemsList[0].WaypointList[0].WaypointTransform.position;
    }

    // moves to next waypoint
    private void toNextWaypoint() {
      // switch to next waypointSystem if no more waypoints in current one
      // stop moving if there's no next waypointsystem
      if (waypointIndex + 1 == rollerCoasterController.WaypointSystemsList[waypointSystemsIndex].WaypointList.Count) {
        // check if there's a next waypoint system
        if (waypointSystemsIndex + 1 == rollerCoasterController.WaypointSystemsList.Count) {
          // if not, stop moving
          isMoving = false;
          return;
        } else {
          // if there is, increment index counter
          waypointSystemsIndex++;
          waypointIndex = 0;
        }
      }

      // get waypoint positions;
      prevWaypointPos = transform.position;
      nextWaypointPos = rollerCoasterController.WaypointSystemsList[waypointSystemsIndex].WaypointList[waypointIndex + 1].WaypointTransform.position;

      // get a normalized vector between wayopints
      currentUnitVectorInter = (prevWaypointPos - nextWaypointPos).normalized;

      // get the segment length to be able to move at correct speed
      currentSegmentLength = Vector3.Distance(prevWaypointPos, nextWaypointPos);

      waypointIndex++;
      timeCounter = 0f;
      isMoving = true;

      // calculate end and start velocity and acceleration when switching waypoints

      // calculate potential energy based on height of next waypoint
      ePot = massCart * accelerationGravity * nextWaypointPos.y;
      // calculate kinetic energy based on difference from total energy
      eKin = eTot - ePot;

      // calculate velocity and acceleration
      // assign last waypoints endvelocity as current start velocity
      currentStartVelocity = currentEndVelocity;
      currentEndVelocity = calculateEndVelocity();
      currentAcceleration = calculateAcceleration();
    }

    private void rotateWheels(float rotationSpeed) {
      wheelsFront.Rotate(new Vector3(-1, 0, 0), rotationSpeed * Time.deltaTime);
      wheelsBack.Rotate(new Vector3(-1, 0, 0), rotationSpeed * Time.deltaTime);
    }

    public void ResetCart() {
       // total energy - we use height of first waypoint
      eTot = massCart * accelerationGravity * rollerCoasterController.WaypointSystemsList[0].WaypointList[0].WaypointTransform.position.y;

      waypointIndex = 0;
      waypointSystemsIndex = 0;
      isMoving = false;
      // speed float translates to meters per second
      currentSpeed = 0.0f;
      timeCounter = 0f;

      currentEndVelocity = 0f;
      currentStartVelocity = 0f;

      isRunning = false;

      transform.rotation = Quaternion.identity;

      setStartPos();
    }

    public void StartStop() {
      if (isRunning) {
        ResetCart();
      } else {
        isRunning = true;
      }
    }
  }
}
