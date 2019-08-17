using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Start Menu (welcome screen) logic
  public class StartMenuController : MonoBehaviour {
    private float startY;
    private float scrollRange;
    private float currentTouchPadY;

    [SerializeField] private GameObject iconSheet;

    void Start() {
      startY = iconSheet.transform.position.y;
      scrollRange = 0.6f;
    }

    void Update() {
      scroll();
    }

    private void scroll() {
      // if not touching, reset values and return (for Go and Quest)

      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
        currentTouchPadY = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch).y;
      } else {
        currentTouchPadY = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;
      }      

      float moveByY = currentTouchPadY * Time.deltaTime * 3f * -1.0f;
      float newPosY = iconSheet.transform.position.y + moveByY;

      // set upper and lower bounds
      if (newPosY > startY && newPosY < startY + scrollRange) {  
        iconSheet.transform.position += new Vector3(0, moveByY, 0);
        }
    }
  }
}
