using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // the Placer System shows potential placing options when player is holding an item
  // that can be placed
  public class PlacerSystem : MonoBehaviour {

    private Dictionary<string, List<PlacerIndicator>> placerIndicatorDict;

    [SerializeField] private PlacerIndicator[] placerIndicatorsArray;

    void Start() {
      // loop through placerIndicatorsArray and save to dictionary
      placerIndicatorDict = new Dictionary<string, List<PlacerIndicator>>();

      for (int i = 0; i < placerIndicatorsArray.Length; i++) {

        string tempIndicatorID = placerIndicatorsArray[i].IndicatorID;

        if (placerIndicatorDict.ContainsKey(tempIndicatorID)) {
          // existing key
          placerIndicatorDict[tempIndicatorID].Add(placerIndicatorsArray[i]);

        } else {
          // new key
          List<PlacerIndicator> tempList = new List<PlacerIndicator>();
          tempList.Add(placerIndicatorsArray[i]);
          placerIndicatorDict.Add(tempIndicatorID, tempList);
        }
        
      }
    }

    public void ShowIndicators(string _indicatorID, bool _show) {

      if (!placerIndicatorDict.ContainsKey(_indicatorID)) return;

      List<PlacerIndicator> tempList = placerIndicatorDict[_indicatorID];

      // loop and activate or deactivate
      for (int i = 0; i < tempList.Count; i++) {
        tempList[i].gameObject.SetActive(_show);
      }
    }

  }
}
