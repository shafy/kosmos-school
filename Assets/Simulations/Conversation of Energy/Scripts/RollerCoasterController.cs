using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // handles RC logic
  public class RollerCoasterController : MonoBehaviour {

    private List<Transform> elementList;

    [SerializeField] private RollerCoasterCart rollerCoasterCart;

    public List<WaypointSystem> WaypointSystemsList;

    public List<Transform> ElementList {
      get { return elementList; }
    }

    void Awake() {
      elementList = new List<Transform>();
      // save all children transforms to list
      foreach (Transform child in transform) {
        elementList.Add(child);
      }
    }

    public void AddElement(Transform element, bool isStartHill = false) {
      // parent element to this
      element.parent = this.transform;
      elementList.Add(element);

      if (isStartHill) {
        // add both sets of waypoints
        WaypointSystem waypointsStart = element.Find("Waypoints (Start)").GetComponent<WaypointSystem>();
        WaypointSystem waypointsFinish = element.Find("Waypoints (Finish)").GetComponent<WaypointSystem>();

        WaypointSystemsList.Add(waypointsStart);
        WaypointSystemsList.Add(waypointsFinish);

        // position at 0 0 0
        element.transform.position = element.parent.transform.position;

        // reset RC cart
        rollerCoasterCart.ResetCart();
      } else {
        // add Waypoints to WaypointSystemsList at second to last position
        WaypointSystem waypoints = element.Find("Waypoints").GetComponent<WaypointSystem>();
        WaypointSystemsList.Insert(WaypointSystemsList.Count - 1, waypoints);
      }
      

    }
  }
}