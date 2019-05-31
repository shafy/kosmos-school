using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Shows or hides a window with text
  public class QuestionBlock : Button {

    [SerializeField] private QuestionBlockController questionBlockController;
    
    public override void Press () {
      questionBlockController.QuestionClick();
    }
  }
}
