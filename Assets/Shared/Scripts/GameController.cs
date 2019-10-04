using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using mixpanel;
using Kosmos.Shared;

namespace Kosmos {
  // general controls for the game
  public class GameController : MonoBehaviour {

    private AudioSource audioSource;
    private bool popUpOpen;
    private GameObject feedbackPopup;

    [SerializeField] private AudioClip openMenuClip;
    [SerializeField] private AudioClip closeMenuClip;
    [SerializeField] private AudioClip showFeedbackPopupClip;
    [SerializeField] private ControllerRayCaster controllerRayCaster;
    [SerializeField] private GameObject ingameMenu;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject feedbackPopupPrefab;

    void Start() {
      ingameMenu.SetActive(false);
      controllerRayCaster.CurrentQuerryTriggerInteraction = QueryTriggerInteraction.Ignore;
      audioSource = GameObject.FindWithTag("UIAudioSource").GetComponent<AudioSource>();

      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
       controllerRayCaster.RayCastEnabled = false;
       controllerRayCaster.EnableLineRenderer(false);
      }

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      Mixpanel.Track("Opened Scene", props);

      popUpOpen = false;

    }

    void Update() {
      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
        if (OVRInput.GetDown(OVRInput.Button.Start)) {
          ToggleIngameMenu();
        }
      } else {
        if (OVRInput.GetDown(OVRInput.Button.Back)) {
          ToggleIngameMenu();
        }
      }
    }

    // void OnApplicationQuit() {
    //   // track timed event end (started in WelcomeGameController)
    //   Mixpanel.Track("App Session");
    // }

    // positions popup at correct distance in front of player
    private void positionPopup(GameObject currentPopup) {
      playerController.HaltUpdateMovement = true;
      // position 2.5 meters in front of user and rotate towards user
      Vector3 newPosMenu = playerController.transform.position + playerController.transform.forward * 2.5f;
      //newPosMenu.y = playerController.transform.position.y ;
      currentPopup.transform.position = newPosMenu;

      Vector3 lookPos = currentPopup.transform.position - playerController.transform.position;
      Quaternion tempRotation = Quaternion.LookRotation(lookPos);
      Quaternion newRotation = Quaternion.Euler(0, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z);
      currentPopup.transform.rotation = newRotation;

      // when menu is activated, raycast doesn't ignore trigger colliders 
      // we use trigger colliders in menu buttons so that if menu is opened over another
      // collider, there is no conflict
      controllerRayCaster.CurrentQuerryTriggerInteraction = QueryTriggerInteraction.Collide;

      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
       controllerRayCaster.RayCastEnabled = true;
       controllerRayCaster.EnableLineRenderer(true);
      }
    }

    private IEnumerator showFeedbackPopupCoroutine(float delaySeconds) {
      yield return new WaitForSeconds(delaySeconds);
      // instantiate feedback popup
      feedbackPopup = (GameObject)Instantiate(feedbackPopupPrefab, new Vector3(0, 0, 0), Quaternion.identity);
      feedbackPopup.GetComponent<FeedbackPopupController>().GameController = this;

      positionPopup(feedbackPopup);

      if (audioSource) {
        audioSource.clip = showFeedbackPopupClip;
        audioSource.Play();
      }

      // set to seen
      PlayerPrefs.SetInt("hasSeenFeedbackPopup", 1);

      // vibrate both controllers
      TouchHaptics.Instance.VibrateFor(0.25f, 0.2f, 0.2f, OVRInput.Controller.Touch);

      popUpOpen = true;

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      Mixpanel.Track("Shown Feedback Popup", props);
    }

    public void ToggleIngameMenu() {

      if (popUpOpen) HideFeedbackPopup();

      if (ingameMenu.activeSelf) {

        if (audioSource) {
          audioSource.clip = closeMenuClip;
          audioSource.Play();
        }
        ingameMenu.SetActive(false);
        playerController.HaltUpdateMovement = false;

        // when menu deactivated, raycast ignores trigger colliders
        controllerRayCaster.CurrentQuerryTriggerInteraction = QueryTriggerInteraction.Ignore;

        if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
         controllerRayCaster.RayCastEnabled = false;
         controllerRayCaster.EnableLineRenderer(false);
        }

      } else {

        ingameMenu.SetActive(true);
        positionPopup(ingameMenu);

        if (audioSource) {
          audioSource.clip = openMenuClip;
          audioSource.Play();
        }

        var props = new Value();
        props["Scene Name"] = SceneManager.GetActiveScene().name;
        Mixpanel.Track("Opened Ingame Menu", props);
      }
    }


    public void ShowFeedbackPopup(float delaySeconds) {
      // only show if it hasn't been shown before
      if (PlayerPrefs.GetInt("hasSeenFeedbackPopup") == 1) return;

      StartCoroutine(showFeedbackPopupCoroutine(delaySeconds));

    }

    public void HideFeedbackPopup() {
      //raycast ignores trigger colliders
      controllerRayCaster.CurrentQuerryTriggerInteraction = QueryTriggerInteraction.Ignore;
      playerController.HaltUpdateMovement = false;

      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
        controllerRayCaster.RayCastEnabled = false;
        controllerRayCaster.EnableLineRenderer(false);
      }

      popUpOpen = false;

      // destroy
      Destroy(feedbackPopup);
    }
  }
}
