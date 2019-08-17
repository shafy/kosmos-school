using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Shows or hides a window with text
  public class QuestionBlock : Button {

    private Collider playerCollider;

    [SerializeField] private QuestionBlockController questionBlockController;

    void OnEnable() {
      Collider collider = GetComponent<Collider>();
      collider.enabled = false;

      StartCoroutine(enableColliderCoroutine(collider));
    }

    void Start() {
      // make sure it doesn't collide with the player collider
      playerCollider = GameObject.FindWithTag("OVRPlayerController").GetComponent<Collider>();
      Physics.IgnoreCollision(playerCollider, GetComponent<Collider>());
    }

    private IEnumerator enableColliderCoroutine(Collider collider) {
      yield return new WaitForSeconds(2f);
      collider.enabled = true;
    }
    
    public override void Press () {
      questionBlockController.QuestionClick();
    }
  }
}
