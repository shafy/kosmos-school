using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // handles roller coaster cart logic
  public class RollerCoasterCart : KosmosPhysics {

    private bool isMoving;
    private float currentSegmentLength;
    private float speed;
    private float timeCounter;
    private float distCovered;
    private float fracSegment;
    private int waypointIndex;
    private int waypointSystemsIndex;
    private Vector3 prevWaypointPos;
    private Vector3 nextWaypointPos;
    private Vector3 currentUnitVectorInter;

    [SerializeField] private WaypointSystem[] waypointSystemsArray;

    void Start() {
      waypointIndex = 0;
      waypointSystemsIndex = 0;
      isMoving = false;
      // speed float translates to meters per second
      speed = 0.6f;
      timeCounter = 0f;

      setStartPos();
    }

    void Update() {

      if (!isMoving) return;

      // calculate what fraction of the segment we've already covered in order to lerp
      timeCounter += Time.deltaTime;
      distCovered = timeCounter * speed;
      fracSegment = distCovered / currentSegmentLength;

      transform.position = Vector3.Lerp(prevWaypointPos, nextWaypointPos, fracSegment);

      // update rotation
      transform.rotation = Quaternion.FromToRotation(transform.forward, currentUnitVectorInter) * transform.rotation;
     
    }

    void OnTriggerEnter(Collider collider) {
      // check name to see if it's a waypoint
      if (!collider.CompareTag("Waypoint")) return;

      toNextWaypoint();
    }

    // sets GameObject to position of first waypoint
    private void setStartPos() {
      transform.position = waypointSystemsArray[0].WaypointList[0].position;
    }

    // moves to next waypoint
    private void toNextWaypoint() {
      // switch to next waypointSystem if no more waypoints in current one
      // stop moving if there's no next waypointsystem
      if (waypointIndex + 1 == waypointSystemsArray[waypointSystemsIndex].WaypointList.Count) {
        // check if there's a next waypoint system
        if (waypointSystemsIndex + 1 == waypointSystemsArray.Length) {
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
      nextWaypointPos = waypointSystemsArray[waypointSystemsIndex].WaypointList[waypointIndex + 1].position;

      // get a normalized vector between wayopints
      currentUnitVectorInter = (nextWaypointPos - prevWaypointPos).normalized;

      // get the segment length to be able to move at correct speed
      currentSegmentLength = Vector3.Distance(prevWaypointPos, nextWaypointPos);
      waypointIndex++;
      timeCounter = 0f;
      isMoving = true;
    }
  }
}
