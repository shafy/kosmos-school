using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // when grabbed, triggers the placer system
  public class PlacerTrigger : MonoBehaviour {

    private PlacerSystem placerSystem;
    private GrabbableHands grabbableHands;

   [SerializeField] private string indicatorID;

    void Start() {
      grabbableHands = GetComponent<GrabbableHands>();
      placerSystem = GameObject.FindWithTag("PlacerSystem").GetComponent<PlacerSystem>();
    }

    // this method is called by GrabbableHands
    public void ShowIndicators(bool _show) {
      placerSystem.ShowIndicators(indicatorID, _show);
    }
  }
}
