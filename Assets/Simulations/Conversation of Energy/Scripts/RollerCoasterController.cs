using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    void Start() {
      rollerCoasterCart.gameObject.SetActive(false);
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

        WaypointSystemsList.Add(waypointsStart);
        WaypointSystemsList.Add(waypointsFinish);

        // position at parent's pos
        element.transform.position = element.parent.transform.position;

        // enable cart
        rollerCoasterCart.gameObject.SetActive(true);
        // reset RC cart
        rollerCoasterCart.ResetCart();
      } else {
        // add Waypoints to WaypointSystemsList at second to last position
        WaypointSystem waypoints = element.Find("Waypoints").GetComponent<WaypointSystem>();
        WaypointSystemsList.Insert(WaypointSystemsList.Count - 1, waypoints);
      }
    }

    public void RemoveElement() {
      Transform lastElement = elementList.Last();
      elementList.RemoveAt(elementList.Count - 1);
      Destroy(lastElement.gameObject);

      // removed start hill in this case
      if (elementList.Count == 0) {
        // clear list
        WaypointSystemsList = new List<WaypointSystem>();
        // hide cart
        rollerCoasterCart.gameObject.SetActive(false);
        return;
      }
      // also remove from waypoinstlist
      WaypointSystemsList.RemoveAt(WaypointSystemsList.Count - 2);
      return;
    }
  }
}
