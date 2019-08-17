using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos
{
  // Allows hands to remotely interact with objects useful for e.g. menus
  public class HandsRemote : MonoBehaviour {

    [SerializeField] private ControllerRayCaster controllerRayCaster;
    [SerializeField] private GameObject destinationPS;
    [SerializeField] private PlayerController playerController;
    
    void Start() {
      setDestinationPS(false);
    }

    public void Update() {
      // stop executing Update if playerController is Frozen
      if (playerController.Frozen) {
        setDestinationPS(true);
        return;
      }

      if (controllerRayCaster.CurrentInteractible) {
        setDestinationPS(true);
      }

      if (!controllerRayCaster.CurrentInteractible || !controllerRayCaster.ShowLineRenderer) {
        setDestinationPS(false);
      }


      // only triggered once one player presses the trigger down
      if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch) && controllerRayCaster.CurrentInteractible) {
        // get current object targeted by raycast
        InteractiveItem currentInteractible = controllerRayCaster.CurrentInteractible;

        // get it's button component
        Button currentButton = currentInteractible.gameObject.GetComponent<Button>();
        if (currentButton == null) return;

        // press the button
        currentButton.Press();
      }
    }

    private void setDestinationPS(bool value) {
      if (value) {
        destinationPS.active = true;
        destinationPS.transform.position = controllerRayCaster.CurrentHit.point;
      } else {
        destinationPS.active = false;
      }
      
    }
  }
}

