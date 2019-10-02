using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos;

namespace Kosmos.Shared {
  // controls logic for feedback popup
  public class FeedbackPopupController : MonoBehaviour {

    [SerializeField] private GameObject stateStart;
    [SerializeField] private GameObject stateYes;
    [SerializeField] private GameObject stateNo;
    [SerializeField] private GameObject stateNotTeacher;

    void Start() {
      setAllActive(false);
      stateStart.SetActive(true);
    }

    private void setAllActive(bool active) {
      stateStart.SetActive(active);
      stateYes.SetActive(active);
      stateNo.SetActive(active);
      stateNotTeacher.SetActive(active);
    }

    private void closePopup() {
      gameObject.SetActive(false);
    }

    private void openBrowser() {
      Application.OpenURL("https://duckduckgo.com");
      closePopup();
    }
 
    public void ButtonPress(string buttonValue) {
      setAllActive(false);
      switch (buttonValue) {
        case "yes":
          stateYes.SetActive(true);
          break;
        case "no":
          stateNo.SetActive(true);
          break;
        case "notteacher":
          stateNotTeacher.SetActive(true);
          break;
        case "yes-yes":
        case "no-yes":
        case "notteacher-yes":
          openBrowser();
          break;
        case "yes-no":
        case "no-no":
        case "notteacher-no":
          closePopup();
          break;
        default:
          stateStart.SetActive(true);
          break;
      }
    }
  }
}
