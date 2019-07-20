using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // handles roller coaster cart logic
  public class RollerCoasterCart : KosmosPhysics {

    private bool isRunning;
    private bool isMoving;
    private bool reverseMode;
    private bool reversingProc;
    private bool syncPlayerController;
    private bool isFinished;
    private float currentSegmentLength;
    private float currentSpeed;
    private float timeCounter;
    private float distCovered;
    private float fracSegment;
    private float prevFracSegment;
    private float accelerationGravity;
    private float eTot;
    private float ePot;
    private float eKin;
    private float currentEndVelocity;
    private float currentStartVelocity;
    private float currentAcceleration;
    private GameObject ovrCameraRig;
    private int waypointIndex;
    private int prevWaypointIndex;
    private int waypointSystemsIndex;
    private PlayerController playerController;
    private Quaternion prevRotation;
    private Quaternion nextRotation;
    private Vector3 prevWaypointPos;
    private Vector3 nextWaypointPos;
    private Vector3 nextUnitVectorInter;

    [SerializeField] private float massCart;
    [SerializeField] private Transform wheelsFront;
    [SerializeField] private Transform wheelsBack;
    [SerializeField] private Transform sensorTop;
    [SerializeField] private Transform sensorBottom;

    public List<WaypointSystem> WaypointSystemsList;

    void Awake() {
      // mass in kg
      accelerationGravity = 9.81f;

      playerController = GameObject.FindWithTag("OVRPlayerController").GetComponent<PlayerController>();
      ovrCameraRig = playerController.transform.Find("OVRCameraRig").gameObject;
    }

    void Update() {

      if (syncPlayerController) {
        //playerController.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        ovrCameraRig.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
      }

      if (isFinished) {
        isFinished = false;
        StartStop();
      }

      if (!isRunning) return; 

      if (!isMoving) return;

      timeCounter += Time.deltaTime;
     
      // calculate speed for current time based on start velocity and acceleration
      currentSpeed = currentStartVelocity + currentAcceleration * timeCounter;

      //currentSpeed = currentSpeed /5;

      // calculate what fraction of the segment we've already covered in order to lerp
      distCovered = timeCounter * currentSpeed;
      fracSegment = distCovered / currentSegmentLength;

      
      if (!reversingProc && prevWaypointIndex == waypointIndex && prevFracSegment > fracSegment) {
        reverseMode = !reverseMode;
        reversingProc = true;
        if (reverseMode) {
          waypointIndex--;
        } else {
          waypointIndex++;
        }
      }

      // update position
      transform.position = Vector3.Lerp(prevWaypointPos, nextWaypointPos, fracSegment);

      transform.rotation = Quaternion.Lerp(prevRotation, nextRotation, fracSegment);

      prevFracSegment = fracSegment;
      prevWaypointIndex = waypointIndex;


      rotateWheels(currentSpeed * 50);

      if (syncPlayerController) {
        //playerController.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        ovrCameraRig.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
      }
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
      transform.position = WaypointSystemsList[0].WaypointList[0].WaypointTransform.position;

      if (syncPlayerController) {
        ovrCameraRig.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
      }
    }

    // moves to next waypoint
    private void toNextWaypoint() {
      if (reversingProc) {
        reversingProc = false;
      }

      if (!reverseMode) {
        // switch to next waypointSystem if no more waypoints in current one
        // stop moving if there's no next waypointsystem
        if (waypointIndex + 1 == WaypointSystemsList[waypointSystemsIndex].WaypointList.Count) {
          // check if there's a next waypoint system
          if (waypointSystemsIndex + 1 == WaypointSystemsList.Count) {
            // if not, stop moving
            isMoving = false;
            isFinished = true;
            return;
          } else {
            // if there is, increment index counter
            waypointSystemsIndex++;
            waypointIndex = 0;
          }
        }
      } else {
        // in this case, we're counting down the waypoints because the cart is going backwards
        // switch to next waypointSystem if no more waypoints in current one
        // stop moving if there's no next waypointsystem
        if (waypointIndex == 0) {
          // check if there's a next waypoint system
          if (waypointSystemsIndex == 0) {
            // if not, stop moving
            isMoving = false;
            isFinished = true;
            return;
          } else {
            // if there is, decrement index counter
            waypointSystemsIndex--;
            waypointIndex = WaypointSystemsList[waypointSystemsIndex].WaypointList.Count;
          }
        }
      }

      // get waypoint positions;
      prevWaypointPos = transform.position;

      prevRotation = Quaternion.FromToRotation(transform.forward, nextUnitVectorInter) * transform.rotation;

      if (!reverseMode) {
        nextWaypointPos = WaypointSystemsList[waypointSystemsIndex].WaypointList[waypointIndex + 1].WaypointTransform.position;
        waypointIndex++;

        // get a normalized vector between wayopints

        nextUnitVectorInter = (nextWaypointPos - prevWaypointPos).normalized;
      } else {
        nextWaypointPos = WaypointSystemsList[waypointSystemsIndex].WaypointList[waypointIndex - 1].WaypointTransform.position;
        waypointIndex--;

        // get a normalized vector between wayopints
        nextUnitVectorInter = (prevWaypointPos - nextWaypointPos).normalized;
      }

      // update rotation
      nextRotation = Quaternion.FromToRotation(transform.forward, nextUnitVectorInter) * transform.rotation;
       // handle z rotation
      Vector3 waypointVec = WaypointSystemsList[waypointSystemsIndex].WaypointList[waypointIndex].WaypointVec;;
      Quaternion tiltRotation = Quaternion.FromToRotation(transform.right, waypointVec) * nextRotation;
      // apply z rotation
      float currentZRotation = tiltRotation.eulerAngles.z;
      nextRotation = Quaternion.Euler(nextRotation.eulerAngles.x, nextRotation.eulerAngles.y, currentZRotation);
      
      // get the segment length to be able to move at correct speed
      currentSegmentLength = Vector3.Distance(prevWaypointPos, nextWaypointPos);

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
      eTot = massCart * accelerationGravity * WaypointSystemsList[0].WaypointList[0].WaypointTransform.position.y;

      waypointIndex = 0;
      waypointSystemsIndex = 0;
      isMoving = false;
      isRunning = false;
      syncPlayerController = false;
      isFinished = false;
      reversingProc = false;
      // speed float translates to meters per second
      currentSpeed = 0.0f;
      timeCounter = 0f;

      currentEndVelocity = 0f;
      currentStartVelocity = 0f;

      reverseMode = false;

      transform.rotation = Quaternion.Euler(0, 180, 0);;

      setStartPos();
    }

    public void StartStop() {
      if (isRunning) {
        ResetCart();
      } else {
        isRunning = true;
      }
    }

    // take player along for the ride :-)
    public void SyncPlayerController(bool enable) {
      ovrCameraRig.transform.parent = transform;
      syncPlayerController = enable;
    }
  }
}
