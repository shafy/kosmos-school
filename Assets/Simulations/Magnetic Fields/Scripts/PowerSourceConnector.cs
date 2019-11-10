using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos.Shared;

namespace Kosmos.MagneticFields {
  // logic for connecting cable heads to power source connectors
  public class PowerSourceConnector : MonoBehaviour {

    private bool isOccupied;
    private ConductorMF occupyingConductorMF;
    private Quaternion powerSourceRotation;

    [SerializeField] private MFController.ConnectorLabel connectorLabel;
    [SerializeField] private MFController mFController;

    public bool IsOccupied {
      get { return isOccupied; }
      set { isOccupied = value; }
    }

    public Quaternion PowerSourceRotation {
      get { return powerSourceRotation; }
    }

    void Start() {
      isOccupied = false;
      powerSourceRotation = mFController.transform.rotation;

      // rotate by 180 if it's an AC connector
      if (connectorLabel == MFController.ConnectorLabel.AC_A1 || connectorLabel == MFController.ConnectorLabel.AC_A2 || connectorLabel == MFController.ConnectorLabel.AC_B1 || connectorLabel == MFController.ConnectorLabel.AC_B2) {
        powerSourceRotation *= Quaternion.Euler(0, 180, 0);
      }
    }

    public void AddConductor(ConductorMF currentConductorMF, ConductorCableHandle.HandleSide currentHandleSide) {
      mFController.AddConductor(currentConductorMF, connectorLabel, currentHandleSide);
      mFController.ReDraw();
    }

    public void RemoveConductor() {
      mFController.RemoveConductor(connectorLabel);
      mFController.ReDraw();
    }

    // checks if this ConductorMF is addable
    public bool IsAddable(ConductorMF _conductorMF, ConductorCableHandle.HandleSide currentHandleSide) {

      if (isOccupied) return false;

      return mFController.IsAddable(_conductorMF, connectorLabel, currentHandleSide);
    }
  }
}
