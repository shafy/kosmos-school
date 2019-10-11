using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // starts and stops current flow
  public class PowerButton : Button {

    [SerializeField] private MFController mFController;

    public override void Press () {
      base.Press();

      mFController.PowerButtonPress();
      
    }
  }
}
