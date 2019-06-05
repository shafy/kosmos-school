using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Start Menu (welcome screen) logic
  public class StartMenuController : MonoBehaviour {

    private float currentTouchPadY;
    private float prevTouchPadY;
    private float startY;
    private float scrollRange;

    [SerializeField] private GameObject iconSheet;

    void Start() {
      resetTouchPadY();
      
      startY = iconSheet.transform.position.y;
      scrollRange = 0.6f;
    }

    void Update() {
      scroll();
    }

    private void scroll() {
      // if not touching, reset values and return
      if (!OVRInput.Get(OVRInput.Touch.PrimaryTouchpad)) {
        resetTouchPadY();
        return;
      }

      currentTouchPadY = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;

      // scroll based on y pos of touchpad
      float scrollDistance = currentTouchPadY - prevTouchPadY;
      if (prevTouchPadY != 0.0f && Mathf.Abs(scrollDistance) > 0.01f) {
        float newPosY = iconSheet.transform.position.y + scrollDistance;

        // set upper and lower bounds
        if (newPosY > startY && newPosY < startY + scrollRange) {
          iconSheet.transform.position = new Vector3(iconSheet.transform.position.x, newPosY, iconSheet.transform.position.z);
        }
        
      }
      prevTouchPadY = currentTouchPadY;
    }

    private void resetTouchPadY() {
      currentTouchPadY = 0.0f;
      prevTouchPadY = 0.0f;
    }
  }
}