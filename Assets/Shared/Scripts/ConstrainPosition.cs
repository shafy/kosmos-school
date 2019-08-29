using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Shared {
  // constrain transform positions and rotations
  // note that you need to add parent GameObject if you don't want to use the
  // world space
  public class ConstrainPosition : MonoBehaviour {

    private float initialPosX;
    private float initialPosY;
    private float initialPosZ;
    private float initialRotX;
    private float initialRotY;
    private float initialRotZ;

    [Header("Position Constraints")]
    [SerializeField] private bool posX;
    [SerializeField] private bool posY;
    [SerializeField] private bool posZ;

    [Header("Rotation Constraints")]
    [SerializeField] private bool rotX;
    [SerializeField] private bool rotY;
    [SerializeField] private bool rotZ;

    void Start() {
      // get initial positions
      initialPosX = transform.localPosition.x;
      initialPosY = transform.localPosition.y;
      initialPosZ = transform.localPosition.z;

      // get initial rotation
      initialRotX = transform.eulerAngles.x;
      initialRotY = transform.eulerAngles.y;
      initialRotZ = transform.eulerAngles.z;
    }

    void Update() {
      // constrain positions
      if (posX) {
        transform.localPosition = new Vector3(initialPosX, transform.localPosition.y, transform.localPosition.z);
      }

      if (posY) {
        transform.localPosition = new Vector3(transform.localPosition.x, initialPosY, transform.localPosition.z);
      }

      if (posZ) {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, initialPosZ);
      }

      if (posX && posZ) {
        transform.localPosition = new Vector3(initialPosX, transform.localPosition.y, initialPosZ);
      }

      if (posX && posY && posZ) {
        transform.localPosition = new Vector3(initialPosX, initialPosY, initialPosZ);
      }
      
      // constrain rotations
      if (rotX && rotY && rotZ) {
        transform.rotation = Quaternion.Euler(initialRotX, initialRotY, initialRotZ);
      }
    }
  }
}
