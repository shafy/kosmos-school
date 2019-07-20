using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // handles Roller Coaster item logic
  public class RollerCoasterItem : MonoBehaviour {

    public enum RCItemType {StartHill, Hill, BankedCurve10, BankedCurve15, BankedCurve20, Looping, Cart};
    [SerializeField] private RCItemType itemType;

    public RCItemType ItemType {
      get { return itemType; }
    }
  }
}