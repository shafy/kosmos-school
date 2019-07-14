using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // starts and stops/resets rollercoaster
  public class RCStartStopButton : Button {

    [SerializeField] private RollerCoasterCart rcCart;


    public override void Press () {
      base.Press();

      rcCart.StartStop();
      
    }
  }
}
