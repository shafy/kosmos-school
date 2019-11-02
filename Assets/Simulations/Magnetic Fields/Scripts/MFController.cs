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
    private bool valueChanged;

    [SerializeField] private SliderControl sliderControl;
    [SerializeField] private TextMeshPro displayText;
    [SerializeField] private TextMeshPro powerText;

    public enum ConnectorLabel {DC_A_POS, DC_A_NEG, DC_B_POS, DC_B_NEG, AC_A1, AC_A2, AC_B1, AC_B2};
    private ConnectorLabel connectorLabel;

    void Start() {
      connectorDict = new Dictionary<ConnectorLabel, ConductorInfo>();
      prevSliderValue = sliderControl.SliderValue;
      powerOn = false;
      valueChanged = false;
      powerText.text = "Power Off";
    }

    void Update() {
      if (valueChanged && !sliderControl.IsGrabbed) {
        if (powerOn) {
          // if new slider value and player let go of slider, set new currents
          setCurrents(sliderControl.SliderValue);
        }

        valueChanged = false;
      }

      if (Mathf.Abs(prevSliderValue - sliderControl.SliderValue) < 0.1) return;

      // update text on display
      displayText.SetText("{0:1}", sliderControl.SliderValue);

      prevSliderValue = sliderControl.SliderValue;
      valueChanged = true;
    }

    // sets currents for currently connected cables
    private void setCurrents(float current) {
      float dcACurrent = 0;
      float dcBCurrent = 0;

      // check if something connected to both connectors of each A DC connector
      if (connectorDict.ContainsKey(ConnectorLabel.DC_A_POS) && connectorDict.ContainsKey(ConnectorLabel.DC_A_NEG)) {
        // also check if the same cable is connected
        if (connectorDict[ConnectorLabel.DC_A_POS].conductorMF == connectorDict[ConnectorLabel.DC_A_NEG].conductorMF) {
          // define in which direction current is flowing
          if (connectorDict[ConnectorLabel.DC_A_POS].handleSide == ConductorCableHandle.HandleSide.right) {
            current = -current;
          }

          // set current
          dcACurrent = current;
          
        }
      }

      // check if something connected to both connectors of each B DC connector
      if (connectorDict.ContainsKey(ConnectorLabel.DC_B_POS) && connectorDict.ContainsKey(ConnectorLabel.DC_B_NEG)) {
        // also check if the same cable is connected
        if (connectorDict[ConnectorLabel.DC_B_POS].conductorMF == connectorDict[ConnectorLabel.DC_B_NEG].conductorMF) {
          // define in which direction current is flowing
          if (connectorDict[ConnectorLabel.DC_B_POS].handleSide == ConductorCableHandle.HandleSide.right) {
            current = -current;
          }

          // set current
          dcBCurrent = current;
        }
      }

      // make sure conductors know about each other
      if (Mathf.Abs(dcACurrent) > 0 && Mathf.Abs(dcBCurrent) > 0) {
        connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetOtherCurrent(dcBCurrent);
        connectorDict[ConnectorLabel.DC_B_POS].conductorMF.SetOtherCurrent(dcACurrent);

        connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetOtherConductor(connectorDict[ConnectorLabel.DC_B_POS].conductorMF.gameObject);
        connectorDict[ConnectorLabel.DC_B_POS].conductorMF.SetOtherConductor(connectorDict[ConnectorLabel.DC_A_POS].conductorMF.gameObject);

      }

      // set currents
      connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetCurrent(dcACurrent);
      connectorDict[ConnectorLabel.DC_B_POS].conductorMF.SetCurrent(dcBCurrent);
    }

    public void AddConductor(ConductorMF _conductorMF, ConnectorLabel _connectorLabel, ConductorCableHandle.HandleSide _handleSide) {

      // check if it exists
      //if (connectorDict.ContainsKey(_connectorLabel)) return;

      connectorDict.Add(_connectorLabel, new ConductorInfo(_conductorMF, _handleSide));
    }

    public void RemoveConductor(ConnectorLabel _connectorLabel) {
      // stop if it doesn't exist
      if (!connectorDict.ContainsKey(_connectorLabel)) return;

      connectorDict.Remove(_connectorLabel);
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

    // checks if this ConductorMF is addable
    public bool IsAddable(ConductorMF _conductorMF, ConnectorLabel _connectorLabel, ConductorCableHandle.HandleSide _handleSide) {

      // check if key exists
      if (connectorDict.ContainsKey(_connectorLabel)) return false;

      // if other handleside is already attached, see if it's on the same connector pair
      switch (_connectorLabel) {
        case ConnectorLabel.DC_A_POS:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_A_NEG)) {
            if (connectorDict[ConnectorLabel.DC_A_NEG].conductorMF != _conductorMF) {
              return false;
            }
          }
          break;
        case ConnectorLabel.DC_A_NEG:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_A_POS)) {
            if (connectorDict[ConnectorLabel.DC_A_POS].conductorMF != _conductorMF) {
              return false;
            }
          }
          break;
        case ConnectorLabel.DC_B_POS:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_B_NEG)) {
            if (connectorDict[ConnectorLabel.DC_B_NEG].conductorMF != _conductorMF) {
              return false;
            }
          }
          break;
        case ConnectorLabel.DC_B_NEG:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_B_POS)) {
            if (connectorDict[ConnectorLabel.DC_B_POS].conductorMF != _conductorMF) {
              return false;
            }
          }
          break;
        case ConnectorLabel.AC_A1:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_A2)) {
            if (connectorDict[ConnectorLabel.AC_A2].conductorMF != _conductorMF) {
              return false;
            }
          }
          break;
        case ConnectorLabel.AC_A2:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_A1)) {
            if (connectorDict[ConnectorLabel.AC_A1].conductorMF != _conductorMF) {
              return false;
            }
          }
          break;
        case ConnectorLabel.AC_B1:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_B2)) {
            if (connectorDict[ConnectorLabel.AC_B2].conductorMF != _conductorMF) {
              return false;
            }
          }
          break;
        case ConnectorLabel.AC_B2:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_B1)) {
            if (connectorDict[ConnectorLabel.AC_B1].conductorMF != _conductorMF) {
              return false;
            }
          }
          break;
      }

      // if another conductor is connected, make sure to only connect same current type

      return true;
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
