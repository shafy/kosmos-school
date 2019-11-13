using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kosmos.Shared;
using TMPro;
using mixpanel;

namespace Kosmos.MagneticFields {
  // takes care of magnetic fields controls
  public class MFController : MonoBehaviour {

    private Dictionary<ConnectorLabel, ConductorInfo> connectorDict;
    private float prevSliderValue;
    private List<float> currentQueue;
    private bool powerOn;
    private bool valueChanged;
    private GameController gameController;

    [SerializeField] private SliderControl sliderControl;
    [SerializeField] private TextMeshPro displayText;
    [SerializeField] private TextMeshPro powerText;
    [SerializeField] private ConductorMF conductorOne;
    [SerializeField] private ConductorMF conductorTwo;
    [SerializeField] private Kosmos.Shared.Light mainLight;

    public enum ConnectorLabel {DC_A_POS, DC_A_NEG, DC_B_POS, DC_B_NEG, AC_A1, AC_A2, AC_B1, AC_B2};
    private ConnectorLabel connectorLabel;

    void Start() {
      connectorDict = new Dictionary<ConnectorLabel, ConductorInfo>();
      currentQueue = new List<float>();
      prevSliderValue = sliderControl.SliderValue;
      powerOn = false;
      valueChanged = false;
      powerText.text = "Power Off";
      gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    void Update() {
      if (valueChanged && !sliderControl.IsGrabbed) {
        if (powerOn) {
          // if new slider value and player let go of slider, set new currents
          //setCurrents(sliderControl.SliderValue);
          currentQueue.Add(sliderControl.SliderValue);
        }

        valueChanged = false;
      }

      // if (Mathf.Abs(prevSliderValue - sliderControl.SliderValue) < 0.1) return;

      if (Mathf.Abs(prevSliderValue - sliderControl.SliderValue) > 0.1) {
        displayText.SetText("{0:1}", sliderControl.SliderValue);

        prevSliderValue = sliderControl.SliderValue;
        valueChanged = true;

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      props["Value"] = sliderControl.SliderValue;
      Mixpanel.Track("Slider", props);
      }

      // update text on display
      // displayText.SetText("{0:1}", sliderControl.SliderValue);

      // prevSliderValue = sliderControl.SliderValue;
      // valueChanged = true;

      if (inProgress() == false) {
        startNextInQueue();
      }
    }

    private void startNextInQueue() {
      if (currentQueue.Count == 0) return;

      setCurrents(currentQueue[0]);
      currentQueue.RemoveAt(0);
    }

    private bool inProgress() {
      return conductorOne.InProgress || conductorTwo.InProgress;
    }

    // sets currents for currently connected cables
    private void setCurrents(float _current) {
      float dcACurrent = 0;
      float dcBCurrent = 0;
      float acACurrent = 0;
      float acBCurrent = 0;

      float currentCurrent = _current;

      // ** DC **

      // check if something connected to both connectors of each A DC connector
      if (connectorDict.ContainsKey(ConnectorLabel.DC_A_POS) && connectorDict.ContainsKey(ConnectorLabel.DC_A_NEG)) {
        // also check if the same cable is connected
        if (connectorDict[ConnectorLabel.DC_A_POS].conductorMF == connectorDict[ConnectorLabel.DC_A_NEG].conductorMF) {
          // check if it's placed on the stand
          if (connectorDict[ConnectorLabel.DC_A_POS].conductorMF.IsPlaced) {
            // define in which direction current is flowing
            if (connectorDict[ConnectorLabel.DC_A_POS].handleSide == ConductorCableHandle.HandleSide.right) {
              dcACurrent = -currentCurrent;
            } else {
              dcACurrent = currentCurrent;
            }  
          }
                  
        }
      }

      // check if something connected to both connectors of each B DC connector
      if (connectorDict.ContainsKey(ConnectorLabel.DC_B_POS) && connectorDict.ContainsKey(ConnectorLabel.DC_B_NEG)) {
        // also check if the same cable is connected
        if (connectorDict[ConnectorLabel.DC_B_POS].conductorMF == connectorDict[ConnectorLabel.DC_B_NEG].conductorMF) {
          // check if it's placed on the stand
          if (connectorDict[ConnectorLabel.DC_B_POS].conductorMF.IsPlaced) {
            // define in which direction current is flowing
            if (connectorDict[ConnectorLabel.DC_B_POS].handleSide == ConductorCableHandle.HandleSide.right) {
              dcBCurrent = -currentCurrent;
            } else {
              dcBCurrent = currentCurrent;
            }
          }
        }
      }

      // make sure conductors know about each other
      if (Mathf.Abs(dcACurrent) > 0 && Mathf.Abs(dcBCurrent) > 0) {
        connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetOtherCurrent(dcBCurrent);
        connectorDict[ConnectorLabel.DC_B_POS].conductorMF.SetOtherCurrent(dcACurrent);

        connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetOtherConductor(connectorDict[ConnectorLabel.DC_B_POS].conductorMF.gameObject);
        connectorDict[ConnectorLabel.DC_B_POS].conductorMF.SetOtherConductor(connectorDict[ConnectorLabel.DC_A_POS].conductorMF.gameObject);
      }

      // remove other conductor (just to be sure if user ran it with two conductors before)
      if (Mathf.Abs(dcACurrent) > 0 && dcBCurrent == 0) {
        connectorDict[ConnectorLabel.DC_A_POS].conductorMF.RemoveOtherConductor();
        connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetOtherCurrent(0);
      }

      if (Mathf.Abs(dcBCurrent) > 0 && dcACurrent == 0) {
        connectorDict[ConnectorLabel.DC_B_POS].conductorMF.RemoveOtherConductor();
        connectorDict[ConnectorLabel.DC_B_POS].conductorMF.SetOtherCurrent(0);
      }

      // set currents for DC
      if (connectorDict.ContainsKey(ConnectorLabel.DC_A_POS)) {
        connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetCurrentType(ConductorMF.CurrentType.dc);
        connectorDict[ConnectorLabel.DC_A_POS].conductorMF.SetCurrent(dcACurrent);
      }

      if (connectorDict.ContainsKey(ConnectorLabel.DC_B_POS)) {
        connectorDict[ConnectorLabel.DC_B_POS].conductorMF.SetCurrentType(ConductorMF.CurrentType.dc);
        connectorDict[ConnectorLabel.DC_B_POS].conductorMF.SetCurrent(dcBCurrent);
      }


      // ** AC **

      // check if something connected to both connectors of each A AC connector
      if (connectorDict.ContainsKey(ConnectorLabel.AC_A1) && connectorDict.ContainsKey(ConnectorLabel.AC_A2)) {
        // also check if the same cable is connected
        if (connectorDict[ConnectorLabel.AC_A1].conductorMF == connectorDict[ConnectorLabel.AC_A2].conductorMF) {
          // check if it's placed on the stand
          if (connectorDict[ConnectorLabel.AC_A1].conductorMF.IsPlaced) {
            // set current
            // for ac it doesn't matter if current is positive or negative
            acACurrent = currentCurrent;
          }
         
        }
      }

      // check if something connected to both connectors of each B AC connector
      if (connectorDict.ContainsKey(ConnectorLabel.AC_B1) && connectorDict.ContainsKey(ConnectorLabel.AC_B2)) {
        // also check if the same cable is connected
        if (connectorDict[ConnectorLabel.AC_B1].conductorMF == connectorDict[ConnectorLabel.AC_B2].conductorMF) {
          // check if it's placed on the stand
          if (connectorDict[ConnectorLabel.AC_B1].conductorMF.IsPlaced) {
            // set current
            // for ac it doesn't matter if current is positive or negative
            acBCurrent = currentCurrent;
          }
          
        }
      }

      // make sure conductors know about each other
      if (Mathf.Abs(acACurrent) > 0 && Mathf.Abs(acBCurrent) > 0) {
        connectorDict[ConnectorLabel.AC_A1].conductorMF.SetOtherCurrent(acBCurrent);
        connectorDict[ConnectorLabel.AC_B1].conductorMF.SetOtherCurrent(acACurrent);

        connectorDict[ConnectorLabel.AC_A1].conductorMF.SetOtherConductor(connectorDict[ConnectorLabel.AC_B1].conductorMF.gameObject);
        connectorDict[ConnectorLabel.AC_B1].conductorMF.SetOtherConductor(connectorDict[ConnectorLabel.AC_A1].conductorMF.gameObject);
      }

      // remove other conductor (just to be sure if user ran it with two conductors before)
      if (Mathf.Abs(acACurrent) > 0 && acBCurrent == 0) {
        connectorDict[ConnectorLabel.AC_A1].conductorMF.RemoveOtherConductor();
        connectorDict[ConnectorLabel.AC_A1].conductorMF.SetOtherCurrent(0);
      }

      if (Mathf.Abs(acBCurrent) > 0 && acACurrent == 0) {
        connectorDict[ConnectorLabel.AC_B1].conductorMF.RemoveOtherConductor();
        connectorDict[ConnectorLabel.AC_B1].conductorMF.SetOtherCurrent(0);
      }

      // set currents for AC
      if (connectorDict.ContainsKey(ConnectorLabel.AC_A1)) {
        connectorDict[ConnectorLabel.AC_A1].conductorMF.SetCurrentType(ConductorMF.CurrentType.ac);
        connectorDict[ConnectorLabel.AC_A1].conductorMF.SetCurrent(acACurrent);
      }

      if (connectorDict.ContainsKey(ConnectorLabel.AC_B1)) {
        connectorDict[ConnectorLabel.AC_B1].conductorMF.SetCurrentType(ConductorMF.CurrentType.ac);
        connectorDict[ConnectorLabel.AC_B1].conductorMF.SetCurrent(acBCurrent);
      }
      
    }

    public void AddConductor(ConductorMF _conductorMF, ConnectorLabel _connectorLabel, ConductorCableHandle.HandleSide _handleSide) {

      // check if it exists
      //if (connectorDict.ContainsKey(_connectorLabel)) return;

      connectorDict.Add(_connectorLabel, new ConductorInfo(_conductorMF, _handleSide));

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      Mixpanel.Track("Add Handle", props);

    }

    public void RemoveConductor(ConnectorLabel _connectorLabel) {

      // stop if it doesn't exist
      if (!connectorDict.ContainsKey(_connectorLabel)) return;

      connectorDict.Remove(_connectorLabel);

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      Mixpanel.Track("Remove Handle", props);
    }

    public void PowerButtonPress() {
      if (!powerOn) {
        powerOn = true;
        powerText.text = "Power On";
        mainLight.TurnOn(true);
        //setCurrents(sliderControl.SliderValue);
        currentQueue.Add(sliderControl.SliderValue);
      } else {
        powerOn = false;
        powerText.text = "Power Off";
        mainLight.TurnOn(false);
        //setCurrents(0);
        currentQueue.Add(0);

        // show feedback popup
        if (gameController) gameController.ShowFeedbackPopup(5);
      }

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      props["Power On"] = powerOn;
      Mixpanel.Track("Power Button Press", props);
    }

    // re-draws magnetic fields
    public void ReDraw() {
      if (!powerOn) return;

      //setCurrents(sliderControl.SliderValue);
      currentQueue.Add(sliderControl.SliderValue);
    }

    // checks if this ConductorMF is addable
    public string IsAddable(ConductorMF _conductorMF, ConnectorLabel _connectorLabel, ConductorCableHandle.HandleSide _handleSide) {

      // check if key exists
      if (connectorDict.ContainsKey(_connectorLabel)) return "error";

      // if other handleside is already attached, see if it's on the same connector pair
      switch (_connectorLabel) {
        case ConnectorLabel.DC_A_POS:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_A_NEG)) {
            if (connectorDict[ConnectorLabel.DC_A_NEG].conductorMF != _conductorMF) {
              return "errorWrongPair";
            }
          }
          break;
        case ConnectorLabel.DC_A_NEG:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_A_POS)) {
            if (connectorDict[ConnectorLabel.DC_A_POS].conductorMF != _conductorMF) {
              return "errorWrongPair";
            }
          }
          break;
        case ConnectorLabel.DC_B_POS:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_B_NEG)) {
            if (connectorDict[ConnectorLabel.DC_B_NEG].conductorMF != _conductorMF) {
              return "errorWrongPair";
            }
          }
          break;
        case ConnectorLabel.DC_B_NEG:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_B_POS)) {
            if (connectorDict[ConnectorLabel.DC_B_POS].conductorMF != _conductorMF) {
              return "errorWrongPair";
            }
          }
          break;
        case ConnectorLabel.AC_A1:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_A2)) {
            if (connectorDict[ConnectorLabel.AC_A2].conductorMF != _conductorMF) {
              return "errorWrongPair";
            }
          }
          break;
        case ConnectorLabel.AC_A2:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_A1)) {
            if (connectorDict[ConnectorLabel.AC_A1].conductorMF != _conductorMF) {
              return "errorWrongPair";
            }
          }
          break;
        case ConnectorLabel.AC_B1:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_B2)) {
            if (connectorDict[ConnectorLabel.AC_B2].conductorMF != _conductorMF) {
              return "errorWrongPair";
            }
          }
          break;
        case ConnectorLabel.AC_B2:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_B1)) {
            if (connectorDict[ConnectorLabel.AC_B1].conductorMF != _conductorMF) {
              return "errorWrongPair";
            }
          }
          break;
      }

      // if another conductor is connected, make sure to only connect same current type
      switch (_connectorLabel) {
        case ConnectorLabel.DC_A_POS:
        case ConnectorLabel.DC_A_NEG:
        case ConnectorLabel.DC_B_POS:
        case ConnectorLabel.DC_B_NEG:
          if (connectorDict.ContainsKey(ConnectorLabel.AC_A1)) return "errorOnlyDC";
          if (connectorDict.ContainsKey(ConnectorLabel.AC_A2)) return "errorOnlyDC";
          if (connectorDict.ContainsKey(ConnectorLabel.AC_B1)) return "errorOnlyDC";
          if (connectorDict.ContainsKey(ConnectorLabel.AC_B2)) return "errorOnlyDC";
          break;
        case ConnectorLabel.AC_A1:
        case ConnectorLabel.AC_A2:
        case ConnectorLabel.AC_B1:
        case ConnectorLabel.AC_B2:
          if (connectorDict.ContainsKey(ConnectorLabel.DC_A_POS)) return "errorOnlyAC";
          if (connectorDict.ContainsKey(ConnectorLabel.DC_A_NEG)) return "errorOnlyAC";
          if (connectorDict.ContainsKey(ConnectorLabel.DC_B_POS)) return "errorOnlyAC";
          if (connectorDict.ContainsKey(ConnectorLabel.DC_B_NEG)) return "errorOnlyAC";
          break;
      }

      return "success";
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
