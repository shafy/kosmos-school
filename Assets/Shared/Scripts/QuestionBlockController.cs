using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kosmos {
  // Main controls of Question Block logic
  public class QuestionBlockController : MonoBehaviour {

    private bool isShowing;
    private bool firstClick;
    private GameObject mainCamera;

    [SerializeField] private bool openAtStart = false;
    [SerializeField] private GameObject windowAndTexts;
    [SerializeField] private TextMeshPro textTitle;
    [SerializeField] private TextMeshPro textBody;
    [SerializeField] private GameObject clickedBlock;
    [SerializeField] private GameObject unclickedBlock;
    [SerializeField] private string titleText;
    [SerializeField, TextArea] private string bodyText;


    void Start() {
      mainCamera = GameObject.FindWithTag("MainCamera");

      isShowing = openAtStart;
      firstClick = !openAtStart;
      showWindow(openAtStart);
      unclickedBlock.active = openAtStart;
      clickedBlock.active = !openAtStart;

      textTitle.text = titleText;
      textBody.text = bodyText;
    }

    private void showWindow(bool value) {
      windowAndTexts.active = value;
      // rotate towards user on Y axis
      Vector3 lookPos =  windowAndTexts.transform.position - mainCamera.transform.position;
      Quaternion tempRotation = Quaternion.LookRotation(lookPos);
      Quaternion newRotation = Quaternion.Euler(0, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z);
      windowAndTexts.transform.rotation = newRotation;
    }

    public void QuestionClick() {
      if (!isShowing) {
        showWindow(true);
        isShowing = true;
        if (firstClick) {
          unclickedBlock.active = false;
          clickedBlock.active = true;
          firstClick = false;
        }
      } else {
        showWindow(false);
        isShowing = false;
      }
    }

  }
}
