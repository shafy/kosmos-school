using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using mixpanel;

namespace Kosmos {
  // general controls for the game
  public class GameController : MonoBehaviour {

    private AudioSource audioSource;

    [SerializeField] private AudioClip openMenuClip;
    [SerializeField] private AudioClip closeMenuClip;
    [SerializeField] private ControllerRayCaster controllerRayCaster;
    [SerializeField] private GameObject ingameMenu;
    [SerializeField] private PlayerController playerController;

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

    public void ToggleIngameMenu() {
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
        playerController.HaltUpdateMovement = true;
        // position 2.5 meters in front of user and rotate towards user
        Vector3 newPosMenu = playerController.transform.position + playerController.transform.forward * 2.5f;
        //newPosMenu.y = playerController.transform.position.y ;
        ingameMenu.transform.position = newPosMenu;

        Vector3 lookPos = ingameMenu.transform.position - playerController.transform.position;
        Quaternion tempRotation = Quaternion.LookRotation(lookPos);
        Quaternion newRotation = Quaternion.Euler(0, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z);
        ingameMenu.transform.rotation = newRotation;

        // when menu is activated, raycast doesn't ignore trigger colliders 
        // we use trigger colliders in menu buttons so that if menu is opened over another
        // collider, there is no conflict
        controllerRayCaster.CurrentQuerryTriggerInteraction = QueryTriggerInteraction.Collide;

        if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
         controllerRayCaster.RayCastEnabled = true;
         controllerRayCaster.EnableLineRenderer(true);
        }

        if (audioSource) {
          audioSource.clip = openMenuClip;
          audioSource.Play();
        }
      }
    }
  }
}
