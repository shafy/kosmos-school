using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kosmos {
  // Main controls of Question Block logic
  public class QuestionBlockController : MonoBehaviour {

    private bool isShowing;
    private bool firstClick;

    [SerializeField] private GameObject infoWindow;
    [SerializeField] private TextMeshPro textTitle;
    [SerializeField] private TextMeshPro textBody;
    [SerializeField] private GameObject clickedBlock;
    [SerializeField] private GameObject unclickedBlock;


    void Start() {
      isShowing = false;
      firstClick = true;
      showWindow(false);
    }

    private void showWindow(bool value) {
      infoWindow.active = value;
      textTitle.gameObject.active = value;
      textBody.gameObject.active = value;
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
