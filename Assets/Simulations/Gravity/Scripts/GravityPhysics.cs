using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Physics behind falling objects
  // object must also have a Rigidbody component
  // this class sets the velocity of the Rigidbody, overriding mass, drag etc.
  public class GravityPhysics : KosmosPhysics {

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
    private float intervalTime;
    private int intervalTimeCounter;
    private List<float> graphDataTime;
    private List<float> graphDataVelocity;

    private Rigidbody rb;

    [SerializeField] private bool isActive = false;
    [SerializeField] private float mass;
    [SerializeField] private float dragCoefficient;
    [SerializeField] private float crossSectionArea;
     // record data every graphTimeStep seconds
    [SerializeField] private float graphTimeStep = 0.5f;
    [SerializeField] private GraphCreator graphCreator;

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

      intervalTime = 0f;
      intervalTimeCounter = 1;      

      // test test test
      // float[] xValues = new float[6] {1f, 2f, 3f, 4f, 5f, 6f};
      // float[] yValues = new float[6] {10f, 5f, 4f, 3f, 2f, 1f};
      //GraphableDataList = new List<GraphableData>();
      // GraphableDataList.Add(new GraphableData(xValues, yValues));
      /// end test

      // initialize data graph stuff
      GraphableDataList = new List<GraphableData>();
      graphDataTime = new List<float>();
      graphDataVelocity = new List<float>();
    }

    void FixedUpdate() {
      // only update velocity if object hasn't collided yet and is active
      if (!isActive) return;

      currentYSpeed = calculateYSpeed();
      prevYSpeed = currentYSpeed;

      currentAcceleration = calculateAcceleration();
      prevAcceleration = currentAcceleration;

      rb.velocity = new Vector3(0, -currentYSpeed, 0);

      // update data every graphTimeStep sec
      intervalTime += Time.deltaTime;
      if (intervalTime > graphTimeStep) {
        intervalTime = 0f;
        // add to graphDataTime list
        graphDataTime.Add(intervalTimeCounter * graphTimeStep);
        intervalTimeCounter++;
        // add to graphDataVelocity list
        graphDataVelocity.Add(currentYSpeed);
      }
    }

    void OnCollisionEnter(Collision collision) {
      // if collision, stop manually updating velocity
      isOnGround = true;
      isActive = false;
      readOnly = false;
      intervalTime = 0f;
      intervalTimeCounter = 1;
      addDataToGraph();
    }

    void OnCollisionExit(Collision collision) {
      // if collision, stop manually updating velocity
      isOnGround = false;
    }

    // calculate current Y speed
    private float calculateYSpeed() {
      return prevAcceleration * Time.deltaTime + prevYSpeed;
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

    private void addDataToGraph() {
      GraphableDataList.Add(new GraphableData(graphDataTime, graphDataVelocity));
      graphCreator.CreateGraph(GraphableDataList);
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
