using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos.Shared;

namespace Kosmos.MagneticFields {
  // logic for connecting cable heads to power source connectors
  public class PowerSourceConnector : MonoBehaviour {

    private bool isOccupied;
    private ConductorMF occupyingConductorMF;

    [SerializeField] private MFController.ConnectorLabel connectorLabel;
    [SerializeField] private MFController mFController;

    void Start() {
      isOccupied = false;
    }

    void OnTriggerEnter(Collider collider) {

      if (occupyingConductorMF) return;

      ConductorCableHandle conductorCableHandle = collider.gameObject.GetComponent<ConductorCableHandle>(); 
      if (!conductorCableHandle) return;

      // position handle
      Transform colliderTrasform = collider.transform;
      colliderTrasform.position = transform.position;

      // add reference to MFController
      ConductorMF currentConductorMF = conductorCableHandle.GetConductorMF();
      mFController.AddConductor(currentConductorMF, connectorLabel);

      occupyingConductorMF = currentConductorMF;
   }

   void OnTriggerExit(Collider collider) {
      ConductorCableHandle conductorCableHandle = collider.gameObject.GetComponent<ConductorCableHandle>(); 
      if (!conductorCableHandle) return;

      occupyingConductorMF = null;
   }
  
  }
}
