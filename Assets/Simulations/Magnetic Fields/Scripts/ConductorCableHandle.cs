using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // logic for the conductor cable's handle
  public class ConductorCableHandle : MonoBehaviour {

    [SerializeField] private ConductorMF currentConductorMF;
  
    public ConductorMF GetConductorMF() {
      return currentConductorMF;
    }  
  
  }
}
