using System.Collections;
using UnityEngine;
using Oculus.Platform;

namespace Kosmos {
  // sets up oculus platform sdk
  public class OculusPlatformSetup : MonoBehaviour {

    private PlayerController playerController;

    [SerializeField] private GameObject errorMessage;

    public bool DoEntitlementCheck = true;

    void Start() {
      playerController = GameObject.FindWithTag("OVRPlayerController").GetComponent<PlayerController>();
      errorMessage.SetActive(false);

      if (!DoEntitlementCheck) return;

      try {
        Core.AsyncInitialize();
        Entitlements.IsUserEntitledToApplication().OnComplete(entitlementCallback);

      } catch (UnityException e) {
        showErrorAndQuit();
      }
    }
    
    private void entitlementCallback(Message msg) {

      // if user not entitled, show error message and quit
      if (msg.IsError) {
        Debug.Log("Entitlement Check failed: " + msg.GetError().Message);
        showErrorAndQuit();
      }
    }

    private void showErrorWindow() {

      errorMessage.SetActive(true);

      // position 2.5 meters in front of user and rotate towards user
      Vector3 newPosError = playerController.transform.position + playerController.transform.forward * 2f;
      newPosError.y = 1.6f;
      errorMessage.transform.position = newPosError;

      Vector3 lookPos = errorMessage.transform.position - playerController.transform.position;
      Quaternion tempRotation = Quaternion.LookRotation(lookPos);
      Quaternion newRotation = Quaternion.Euler(0, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z);
      errorMessage.transform.rotation = newRotation;
      return;
    }

    private void showErrorAndQuit() {
      // disable ray caster so user can't click on a sim
      playerController.Frozen = true;
      showErrorWindow();
      StartCoroutine(quitApp());
    }

    private IEnumerator quitApp() {
      yield return new WaitForSeconds(8.0f);
      UnityEngine.Application.Quit();
    }
  }
}
