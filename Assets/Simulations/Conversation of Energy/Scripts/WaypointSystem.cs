using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // handles waypoint system logic
  public class WaypointSystem : MonoBehaviour {

    private List<Transform> waypointList;

    public List<Transform> WaypointList {
      get { return waypointList; }
    }

    void Awake() {
      waypointList = new List<Transform>();
      // save all children transforms to list
      foreach (Transform child in transform) {
        Mesh mesh = child.GetComponent<MeshFilter>().mesh;
        waypointList.Add(child);
      }

      // Mesh mesh = waypointList[0].GetComponent<MeshFilter>().mesh;
      // Debug.Log("normals 0:" + mesh.normals[0]);
      

      // Mesh mesh15 = waypointList[15].GetComponent<MeshFilter>().mesh;
      // Debug.Log("normals 15:" + mesh15.normals[0]);
      
      // Debug.Log("Angle 0" + Vector3.Angle(mesh.normals[0], new Vector3(0, 1, 0)));
      // Debug.Log("Angle 15" + Vector3.Angle(mesh15.normals[0], new Vector3(0, 1, 0)));
       
    }
  }

}