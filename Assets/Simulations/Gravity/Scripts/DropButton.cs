using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Drops the object (activates GravityPhysics)
  public class DropButton : Button {

    private bool shouldMove;
    private float placeHeight;
    private Vector3 endPos;

    [SerializeField] private GravityPhysics currentGP;

    public override void Press () {
      // drop
      currentGP.SetActive(true);
    }

  }
}
