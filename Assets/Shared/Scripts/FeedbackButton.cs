using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos;

namespace Kosmos.Shared {
  // when user clicks on a Popup Feedback Button
  public class FeedbackButton : TextureButton {

    [SerializeField] private string buttonValue;
    [SerializeField] private FeedbackPopupController feedbackPopupController;
    
    public override void Press () {
      base.Press();
      feedbackPopupController.ButtonPress(buttonValue);
    }
  }
}
