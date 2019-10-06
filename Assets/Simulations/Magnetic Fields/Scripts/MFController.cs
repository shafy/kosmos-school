using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos.Shared;

namespace Kosmos.MagneticFields {
  // takes care of magnetic fields controls
  public class MFController : MonoBehaviour {

    private Dictionary<ConnectorLabel, ConductorMF> connectorDict;
    private float prevSliderValue;

    [SerializeField] private SliderControl sliderControl;
    //[SerializeField] private ConductorMF conductorMF;

    public enum ConnectorLabel {DC_A1, DC_A2, DC_B1, DC_B2, AC_A1, AC_A2, AC_B1, AC_B2};
    private ConnectorLabel connectorLabel;

    void Start() {
      prevSliderValue = sliderControl.SliderValue;
    }

    void Update() {
      if (prevSliderValue == sliderControl.SliderValue) return;

      //conductorMF.SetCurrent(sliderControl.SliderValue);
      prevSliderValue = sliderControl.SliderValue;
    }

    public void AddConductor(ConductorMF _conductorMF, ConnectorLabel _connectorLabel) {

      // check if it exists
      if (connectorDict.ContainsKey(_connectorLabel)) return;

      connectorDict.Add(_connectorLabel, _conductorMF);
    }

    public void PowerButtonPress() {

    }
  
  }
}
