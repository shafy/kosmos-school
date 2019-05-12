using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Physics behind falling objects
  // object must also have a Rigidbody component
  // this class sets the velocity of the Rigidbody, overriding mass, drag etc.
  public class GravityPhysics : MonoBehaviour {

    private bool isOnGround;
    private bool readOnly;
    private float airDensity;
    private float currentYSpeed;
    private float prevYSpeed;
    private float currentAcceleration;
    private float initialAcceleration;
    private float prevAcceleration;
    private float accelerationG;
    private float currentFixedTime;

    private Rigidbody rb;

    [SerializeField] private bool isActive = true;
    [SerializeField] private float mass;
    [SerializeField] private float dragCoefficient;
    [SerializeField] private float crossSectionArea;

    public bool ReadOnly {
      get { return readOnly; }
      private set { readOnly = value; }
    }

    void Start() {
      rb = GetComponent<Rigidbody>();
      initialAcceleration = 9.81f;
      prevAcceleration = currentAcceleration = initialAcceleration;

      airDensity = 1.229f; 
      accelerationG = 9.81f;

      isOnGround = false;

      if (!isActive) {
        rb.isKinematic = true;
      }

      readOnly = false;
    }

    void FixedUpdate() {
      // only update velocity if object hasn't collided yet and is active
      if (!isActive) return;

      currentYSpeed = calculateYSpeed();
      prevYSpeed = currentYSpeed;

      currentAcceleration = calculateAcceleration();
      prevAcceleration = currentAcceleration;

      rb.velocity = new Vector3(0, -currentYSpeed, 0);
    }

    void OnCollisionEnter(Collision collision) {
      // if collision, stop manually updating velocity
      isOnGround = true;
      isActive = false;
      readOnly = false;
    }

    void OnCollisionExit(Collision collision) {
      // if collision, stop manually updating velocity
      isOnGround = false;
    }

    // calculate current Y speed
    private float calculateYSpeed() {
      return prevAcceleration * Time.fixedDeltaTime + prevYSpeed;
    }

    // calculate current acceleration
    private float calculateAcceleration() {
      return fRes() / mass;
    }

    // calculate F_g (force due to gravity)
    private float fGravity() {
      return accelerationG * mass;
    }

    // calculate F_air (force due to air resistance)
    private float fAir() {
      return 0.5f * dragCoefficient * Mathf.Pow(currentYSpeed, 2.0f) * crossSectionArea * airDensity;
    }

    // calculate resulting force
    private float fRes() {
      return fGravity() - fAir();
    }

    public void SetActive(bool value) {
      // can only change active status if not readOnly
      if (readOnly) return;
      // can't activate if it's already on the ground
      if (value == true && isOnGround) return;
      isActive = value;
      rb.isKinematic = !value;
      readOnly = true;
    }

    // sets this object read only
    public void SetReadOnly(bool value) {
      readOnly = value;
    }
  }
}
