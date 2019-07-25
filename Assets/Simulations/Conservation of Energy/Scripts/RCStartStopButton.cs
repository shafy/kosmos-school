using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // starts and stops/resets rollercoaster
  public class RCStartStopButton : Button {

    [SerializeField] private RollerCoasterBuilderController rollerCoasterBuilderController;


    public override void Press () {
      base.Press();

      rollerCoasterBuilderController.StartStopCart();
      
    }
  }
}
