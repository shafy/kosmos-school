using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kosmos {
  // handles RC logic
  public class RollerCoasterController : MonoBehaviour {

    private List<Transform> elementList;
    private RollerCoasterCart rollerCoasterCart;

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

    private void syncWaypointsWithCart() {
      if (!rollerCoasterCart) return;
      rollerCoasterCart.WaypointSystemsList = WaypointSystemsList;
    }

    public void StartStopCart() {
      rollerCoasterCart.StartStop();
    }

    public void AddElement(Transform element, bool isStartHill = false) {
      // parent element to this
      element.parent = this.transform;
      elementList.Add(element);

      if (isStartHill) {
        // add both sets of waypoints
        WaypointSystem waypointsStart = element.Find("Waypoints (Start)").GetComponent<WaypointSystem>();
        WaypointSystem waypointsFinish = element.Find("Waypoints (Finish)").GetComponent<WaypointSystem>();

        waypointsStart.SetupWaypoints();
        waypointsFinish.SetupWaypoints();

        WaypointSystemsList.Add(waypointsStart);
        WaypointSystemsList.Add(waypointsFinish);

        // position at parent's pos
        element.position = element.parent.transform.position;
        // rotate
        element.rotation = this.transform.rotation;

      } else {
        // add Waypoints to WaypointSystemsList at second to last position
        Transform waypointsTranform = element.Find("Waypoints");

        if (!waypointsTranform) return;

        WaypointSystem waypoints = waypointsTranform.GetComponent<WaypointSystem>();
        waypoints.SetupWaypoints();
        WaypointSystemsList.Insert(WaypointSystemsList.Count - 1, waypoints);
      }

      syncWaypointsWithCart();
    }

    public void RemoveElement() {
      Transform lastElement = elementList.Last();
      elementList.RemoveAt(elementList.Count - 1);
      Destroy(lastElement.gameObject);

      // removed start hill in this case
      if (elementList.Count == 0) {
        // clear list
        WaypointSystemsList = new List<WaypointSystem>();
        // destroy cart
        if (rollerCoasterCart) Destroy(rollerCoasterCart.gameObject);
        //rollerCoasterCart.gameObject.SetActive(false);
        return;
      }
      // also remove from waypoinstlist
      WaypointSystemsList.RemoveAt(WaypointSystemsList.Count - 2);
      syncWaypointsWithCart();
      return;
    }

    public void AddCartReference(RollerCoasterCart _rollerCoasterCart) {
      rollerCoasterCart = _rollerCoasterCart;
      syncWaypointsWithCart();
      rollerCoasterCart.ResetCart();
    }

    public bool IsRunning() {
      if (!rollerCoasterCart) return false;
      return rollerCoasterCart.IsRunning;
    }
  }
}
