using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kosmos;
using mixpanel;

namespace Kosmos.Shared {
  // controls logic for feedback popup
  public class FeedbackPopupController : MonoBehaviour {

    private GameController gameController;

    [SerializeField] private GameObject stateStart;
    [SerializeField] private GameObject stateYes;
    [SerializeField] private GameObject stateNo;
    [SerializeField] private GameObject stateNotTeacher;

    public GameController GameController {
      get { return gameController; }
      set { gameController = value; }
    }

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
      gameController.HideFeedbackPopup();
    }

    private void openBrowser(string url) {
      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      Mixpanel.Track("Opened Survey", props);

      Application.OpenURL(url);
      closePopup();
    }
 
    public void ButtonPress(string buttonValue) {
      setAllActive(false);

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      props["Button Name"] = buttonValue;
      Mixpanel.Track("Clicked Survey Button", props);

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
          openBrowser("https://airtable.com/shr5sUoX58vXreO2F?prefill_teacher=Yes");
          break;
        case "no-yes":
          openBrowser("https://airtable.com/shr5sUoX58vXreO2F");
          break;
        case "notteacher-yes":
          openBrowser("https://airtable.com/shr5sUoX58vXreO2F?prefill_teacher=No");
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
