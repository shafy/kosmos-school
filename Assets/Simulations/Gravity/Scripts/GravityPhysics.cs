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
    private GraphCreator graphCreator;
    private int intervalTimeCounter;
    private List<float> graphDataTime;
    private List<float> graphDataSpeed;
    private List<float> graphDataAcceleration;
    private Rigidbody rb;

    [SerializeField] private bool isActive = false;
    [SerializeField] private float mass;
    [SerializeField] private float dragCoefficient;
    [SerializeField] private float crossSectionArea;
     // record data every graphTimeStep seconds
    [SerializeField] private float graphTimeStep = 0.5f;
    [SerializeField] private PlatformController platformController;
    [SerializeField] private string dataName;
    [SerializeField] private Color dataColor;

    public bool ReadOnly {
      get { return readOnly; }
      private set { readOnly = value; }
    }

    public bool IsActive {
      get { return isActive; }
      private set { isActive = value; }
    }

    void Awake() {
      graphCreator = platformController.GraphCreator;
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

      // initialize data graph stuff
      //GraphableDataList = new List<GraphableData>();
      graphDataTime = new List<float>();
      graphDataSpeed = new List<float>();
      graphDataAcceleration = new List<float>();
      //GraphableDescriptionList = new List<GraphableDescription>();
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
        // add to graphDataSpeed list
        graphDataSpeed.Add(currentYSpeed);
        // add acceleration to graphDataAcceleration
        graphDataAcceleration.Add(currentAcceleration);
      }
    }

    void OnCollisionEnter(Collision collision) {
      // if collision, stop manually updating velocity
      // only care about collisions with PlatformFloor and that are actively falling
      if (!collision.gameObject.CompareTag("PlatformFloor") || !isActive) return;
      isOnGround = true;
      isActive = false;
      //rb.isKinematic = true;
      readOnly = false;
      intervalTime = 0f;
      intervalTimeCounter = 1;
      addDataToGraph();
      // let platformController now that the object has dropped
      platformController.DropComplete(true);
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
      if (!graphCreator || graphDataTime.Count == 0) return;

      // add velocity graph
      //GraphableDataList.Add(new GraphableData(graphDataTime, graphDataSpeed));
      //GraphableDescriptionList.Add(new GraphableDescription("Speed", "Speed", "Time [s]", "Speed [m/s]"));
      // add acceleration graph
      //GraphableDataList.Add(new GraphableData(graphDataTime, graphDataAcceleration));
      //GraphableDescriptionList.Add(new GraphableDescription("Acceleration", "Acceleration", "Time [s]", "Acceleration [m/s^2]"));
      // new Color(1f, 1f, 0.6f)
      // new Color(0.6f, 0.99f, 0.88f)

      //graphCreator.CreateGraph(GraphableDataList, GraphableDescriptionList);
      graphCreator.AddToDataSet(new GraphableData(graphDataTime, graphDataSpeed, dataName, dataColor), "Speed");
      graphCreator.AddToDataSet(new GraphableData(graphDataTime, graphDataAcceleration, dataName, dataColor), "Acceleration");      
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
