using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // logic for the conductor cable's handle
  public class ConductorCableHandle : MonoBehaviour {

    [SerializeField] private ConductorMF currentConductorMF;

    public enum HandleSide {right, left};
    [SerializeField] private HandleSide currentHandleSide;

    public HandleSide CurrentHandleSide {
      get { return currentHandleSide; }
    }
  
    public ConductorMF GetConductorMF() {
      return currentConductorMF;
    }  
  
  }
}
