using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // handles logic for tooltip system
  public class ToolTipSystem : MonoBehaviour {

    [SerializeField] private GameObject[] toolTipsArray;

    public void ShowToolTip(string name, bool show) {
      // disable or enable tooltip with specific name
      for (int i = 0; i < toolTipsArray.Length; i++) {
        if (toolTipsArray[i].name == name) toolTipsArray[i].SetActive(show);
      }
    }
  }
}
