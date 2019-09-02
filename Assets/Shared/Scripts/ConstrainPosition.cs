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
    private Vector3 newLocalPos;

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
      transform.localPosition = Constrain();

      // constrain rotations
      if (rotX && rotY && rotZ) {
        transform.rotation = Quaternion.Euler(initialRotX, initialRotY, initialRotZ);
      }
    }

    public Vector3 Constrain() {
      // constrain positions
      if (posX) {
        newLocalPos = new Vector3(initialPosX, transform.localPosition.y, transform.localPosition.z);
      }

      if (posY) {
        newLocalPos = new Vector3(transform.localPosition.x, initialPosY, transform.localPosition.z);
      }

      if (posZ) {
        newLocalPos = new Vector3(transform.localPosition.x, transform.localPosition.y, initialPosZ);
      }

      if (posX && posZ) {
        newLocalPos = new Vector3(initialPosX, transform.localPosition.y, initialPosZ);
      }

      if (posX && posY && posZ) {
        newLocalPos = new Vector3(initialPosX, initialPosY, initialPosZ);
      }

      return newLocalPos;
    }
  }
}
