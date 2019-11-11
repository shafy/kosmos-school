using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos.Shared;

namespace Kosmos.MagneticFields {
  // starts and stops current flow
  public class PowerButton : Button {

    private ToolTipSystem tooltipSystem;

    [SerializeField] private MFController mFController;

    void Start() {
      base.Start();

      tooltipSystem = GameObject.FindWithTag("TooltipSystem").GetComponent<ToolTipSystem>();
    }

    public override void Press () {
      base.Press();

      mFController.PowerButtonPress();

      // hide tooltip
      tooltipSystem.ShowToolTip("Tooltip MF", false);
      
    }
  }
}
