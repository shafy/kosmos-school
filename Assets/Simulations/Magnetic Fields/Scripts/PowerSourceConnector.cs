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
    }

    public void AddConductor(ConductorMF currentConductorMF, ConductorCableHandle.HandleSide currentHandleSide) {
      mFController.AddConductor(currentConductorMF, connectorLabel, currentHandleSide);
    }

    public void RemoveConductor() {
      mFController.RemoveConductor(connectorLabel);
    }
  }
}
