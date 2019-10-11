using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos.Shared;
using TMPro;

namespace Kosmos.MagneticFields {
  // takes care of magnetic fields controls
  public class MFController : MonoBehaviour {

    private Dictionary<ConnectorLabel, ConductorInfo> connectorDict;
    private float prevSliderValue;
    private bool powerOn;

    [SerializeField] private SliderControl sliderControl;
    [SerializeField] private TextMeshPro displayText;
    [SerializeField] private TextMeshPro powerText;

    public enum ConnectorLabel {DC_A_POS, DC_A_NEG, DC_B_POS, DC_B_NEG, AC_A1, AC_A2, AC_B1, AC_B2};
    private ConnectorLabel connectorLabel;

    void Start() {
      connectorDict = new Dictionary<ConnectorLabel, ConductorInfo>();
      prevSliderValue = sliderControl.SliderValue;
      powerOn = false;
      powerText.text = "Power Off";
    }

    void Update() {
      //if (prevSliderValue == sliderControl.SliderValue) return;
      if ( Mathf.Abs(prevSliderValue - sliderControl.SliderValue) < 0.1) return;

      // update text on display
      displayText.SetText("{0:1}", sliderControl.SliderValue);

      if (!sliderControl.IsGrabbed && powerOn) {
        // if new slider value and player let go of slider, set new currents
        setCurrents(sliderControl.SliderValue);
      }

      prevSliderValue = sliderControl.SliderValue;
    }

    // sets currents for currently connected cables
    private void setCurrents(float current) {
      // check if something connected to both connectors of each A and B
      // also check if the same cable is connected
      if (connectorDict.ContainsKey(ConnectorLabel.DC_A_POS) && connectorDict.ContainsKey(ConnectorLabel.DC_A_NEG)) {
        if (connectorDict[ConnectorLabel.DC_A_POS].conductorMF == connectorDict[ConnectorLabel.DC_A_NEG].conductorMF) {
          // define in which direction current is flowing
          if (connectorDict[ConnectorLabel.DC_A_POS].handleSide == ConductorCableHandle.HandleSide.right) {
            current = -current;
          }

          // set current
          connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetCurrent(current);
        }
      }
    }

    public void AddConductor(ConductorMF _conductorMF, ConnectorLabel _connectorLabel, ConductorCableHandle.HandleSide _handleSide) {

      // check if it exists
      if (connectorDict.ContainsKey(_connectorLabel)) return;

      connectorDict.Add(_connectorLabel, new ConductorInfo(_conductorMF, _handleSide));
    }

    public void PowerButtonPress() {
      if (!powerOn) {
        powerOn = true;
        powerText.text = "Power On";
        setCurrents(sliderControl.SliderValue);
      } else {
        powerOn = false;
        powerText.text = "Power Off";
        setCurrents(0);
      }

      
      
    }
  
  }

  // struct used in connectorDict
  public struct ConductorInfo {
    public ConductorMF conductorMF;
    public ConductorCableHandle.HandleSide handleSide;

    public ConductorInfo(ConductorMF _conductorMF, ConductorCableHandle.HandleSide _handleSide) {
      conductorMF = _conductorMF;
      handleSide = _handleSide;
    }
  }
}
