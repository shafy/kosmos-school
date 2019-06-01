using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // general controls for the game
  public class GameController : MonoBehaviour {

    [SerializeField] private GameObject ingameMenu;
    [SerializeField] private PlayerController playerController;

    void Start() {
      ingameMenu.SetActive(false);
    }

    void Update() {
      if (OVRInput.GetDown(OVRInput.Button.Back)) {
        ToggleIngameMenu();
      }
    }

    public void ToggleIngameMenu() {
      if (ingameMenu.activeSelf) {
        ingameMenu.SetActive(false);
        playerController.WalkingAllowed = true;
      } else {
        ingameMenu.SetActive(true);
        playerController.WalkingAllowed = false;
        // position 3 meters in front of user and rotate towards user
        Vector3 newPosMenu = playerController.transform.position + playerController.transform.forward * 2;
        newPosMenu.y = 0.0f;
        ingameMenu.transform.position = newPosMenu;

        Vector3 lookPos = ingameMenu.transform.position - playerController.transform.position;
        Quaternion tempRotation = Quaternion.LookRotation(lookPos);
        Quaternion newRotation = Quaternion.Euler(0, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z);
        ingameMenu.transform.rotation = newRotation;
      }
    }
  }
}