using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void Update() {
      if (OVRInput.GetDown(OVRInput.Button.Back)) {
        ToggleIngameMenu();
        
      }
    }

    public void ToggleIngameMenu() {
      if (ingameMenu.activeSelf) {

        if (audioSource) {
          audioSource.clip = closeMenuClip;
          audioSource.Play();
        }
        ingameMenu.SetActive(false);
        playerController.WalkingAllowed = true;

        // when menu deactivated, raycast ignores trigger colliders
        controllerRayCaster.CurrentQuerryTriggerInteraction = QueryTriggerInteraction.Ignore;

      } else {

        ingameMenu.SetActive(true);
        playerController.WalkingAllowed = false;
        // position 2.5 meters in front of user and rotate towards user
        Vector3 newPosMenu = playerController.transform.position + playerController.transform.forward * 2.5f;
        newPosMenu.y = 1.3f;
        ingameMenu.transform.position = newPosMenu;

        Vector3 lookPos = ingameMenu.transform.position - playerController.transform.position;
        Quaternion tempRotation = Quaternion.LookRotation(lookPos);
        Quaternion newRotation = Quaternion.Euler(0, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z);
        ingameMenu.transform.rotation = newRotation;

        // when menu is activated, raycast doesn't ignore trigger colliders 
        // we use trigger colliders in menu buttons so that if menu is opened over another
        // collider, there is no conflict
        controllerRayCaster.CurrentQuerryTriggerInteraction = QueryTriggerInteraction.Collide;

        if (audioSource) {
          audioSource.clip = openMenuClip;
          audioSource.Play();
        }
        

      }
    }
  }
}