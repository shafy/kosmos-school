using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kosmos {
  // handles waypoint system logic
  public class WaypointSystem : MonoBehaviour {

    private List<Transform> allChildren;
    private List<Waypoint> waypointList;

    public List<Waypoint> WaypointList {
      get { return waypointList; }
    }

    void Awake() {
      allChildren = new List<Transform>();
      waypointList = new List<Waypoint>();
      // save all children transforms to list (without getting current gameobject)
      foreach (Transform child in transform) {
        allChildren.Add(child);
      }

      for (int i = 0; i < allChildren.Count; i = i + 3) {
        // there are three gameobjects per waypoint. we need to get a vector between the first 2
        // in order to know the "tilt" of the roller coaster tracks
        // and we need to add a spherecollider to the third

        Vector3 firstPos = allChildren[i].position;
        Vector3 secondPos = allChildren[i+1].position;
        Vector3 firstToSecondVec = (firstPos - secondPos).normalized;

        // add sphere collider
        Transform thirdPos = allChildren[i+2];
        addSphereCollider(thirdPos.gameObject);

        waypointList.Add(new Waypoint(thirdPos, firstToSecondVec));
      }
    }

    private void addSphereCollider(GameObject currentGO) {
      SphereCollider sc = currentGO.AddComponent<SphereCollider>() as SphereCollider;
      sc.radius = 0.1f;
      sc.center = new Vector3(0, 0, 0);
      sc.isTrigger = true;
      sc.gameObject.tag = "Waypoint";
    }
  }

  public class Waypoint {
    private Transform waypointTransform;
    private Vector3 waypointVec;

    public Transform WaypointTransform {
      get { return waypointTransform; }
      private set { waypointTransform = value; }
    }

    public Vector3 WaypointVec {
      get { return waypointVec; }
      private set { waypointVec = value; }
    }

    public Waypoint(Transform _waypointTransform, Vector3 _waypointVec) {
      waypointTransform = _waypointTransform;
      waypointVec = _waypointVec;
    }
  }
}
