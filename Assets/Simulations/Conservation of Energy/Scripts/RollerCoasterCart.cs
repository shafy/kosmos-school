using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // handles roller coaster cart logic
  public class RollerCoasterCart : KosmosPhysics {

    private AudioSource audioSourceInstantiate;
    private AudioSource audioSourceCountdown;
    private bool isRunning;
    private bool isMoving;
    private bool reverseMode;
    private bool reversingProc;
    private bool syncPlayerController;
    private bool timeZeroAdded;
    private bool physicsActive;
    private bool trackIsComplete;
    private float currentSegmentLength;
    private float currentSpeed;
    private float timeCounter;
    private float distCovered;
    private float fracSegment;
    private float prevFracSegment;
    private float accelerationGravity;
    private float eTot;
    private float ePot;
    private float eKin;
    private float eKinStart;
    private float currentEndVelocity;
    private float currentStartVelocity;
    private float currentAcceleration;
    private float intervalTime;
    private float tracksMinHeight;
    private List<float> graphDataTime;
    private List<float> graphDataEKin;
    private List<float> graphDataEPot;
    private List<float> graphDataETot;
    private List<float> graphDataAcceleration;
    private List<float> graphDataSpeed;
    private List<float> graphDataHeight;
    private GameObject ovrCameraRig;
    private GameObject playerControllerGO;
    private GraphCreator graphCreator;
    private int waypointIndex;
    private int prevWaypointIndex;
    private int waypointSystemsIndex;
    private int intervalTimeCounter;
    private Quaternion prevRotation;
    private Quaternion nextRotation;
    private Quaternion ovrCameraPrevRot;
    private PlayerController playerController;
    private Rigidbody rb;
    private RollerCoasterBuilderController rollerCoasterBuilderController;
    private Vector3 prevWaypointPos;
    private Vector3 nextWaypointPos;
    private Vector3 nextUnitVectorInter;

    [SerializeField] private BoxCollider fallBoxCollider;
    [SerializeField] private Color dataColor;
    [SerializeField] private float massCart;
    [SerializeField] private float graphTimeStep = 0.5f;
    [SerializeField] private string dataName;
    [SerializeField] private Transform wheelsFront;
    [SerializeField] private Transform wheelsBack;
    [SerializeField] private Transform sensorTop;
    [SerializeField] private Transform sensorBottom;
    [SerializeField] private GameObject particleSystemPrefab;
    [SerializeField] private AudioClip audioClipInstantiate;
    [SerializeField] private AudioClip audioClipCountDown;


    public List<WaypointSystem> WaypointSystemsList;

    public bool IsRunning {
      get { return isRunning; }
    }

    void Awake() {
      // mass in kg
      accelerationGravity = 9.81f;
      tracksMinHeight = 3.66f;
      eKinStart = massCart * 5;

      playerControllerGO = GameObject.FindWithTag("OVRPlayerController");
      playerController = playerControllerGO.GetComponent<PlayerController>();
      ovrCameraRig = playerControllerGO.transform.Find("OVRCameraRig").gameObject;

      audioSourceInstantiate = gameObject.AddComponent<AudioSource>();
      audioSourceInstantiate.playOnAwake = false;
      audioSourceInstantiate.volume = 0.5f;
      audioSourceInstantiate.clip = audioClipInstantiate;

      audioSourceCountdown = gameObject.AddComponent<AudioSource>();
      audioSourceCountdown.playOnAwake = false;
      audioSourceCountdown.volume = 0.5f;
      audioSourceCountdown.clip = audioClipCountDown;

      rb = GetComponent<Rigidbody>();
      fallBoxCollider.enabled = false;

      rollerCoasterBuilderController = GameObject.Find("RollerCoasterBuilder").GetComponent<RollerCoasterBuilderController>();

      isRunning = false;
      isMoving = false;
    }

    void Update() {

      if (syncPlayerController && !isRunning) {
        ovrCameraRig.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
      }

      if (physicsActive) return;

      if (!isRunning) return; 

      if (!isMoving) return;

      if (!timeZeroAdded) {
        timeZeroAdded = true;

        graphDataTime.Add(0f);
        graphDataEKin.Add(eKinStart / 1000);
        // we add eTot for ePot on purpose
        graphDataEPot.Add(eTot / 1000);
        graphDataETot.Add((eTot + eKinStart) / 1000);
        graphDataAcceleration.Add(0f);
        graphDataSpeed.Add(0f);
        graphDataHeight.Add(WaypointSystemsList[0].WaypointList[0].WaypointTransform.position.y - tracksMinHeight);
      }

      timeCounter += Time.deltaTime;
     
      // calculate speed for current time based on start velocity and acceleration
      currentSpeed = currentStartVelocity + currentAcceleration * timeCounter;

      //currentSpeed = currentSpeed / 2;

      // calculate what fraction of the segment we've already covered in order to lerp
      distCovered = timeCounter * currentSpeed;
      fracSegment = distCovered / currentSegmentLength;

      
      if (!reversingProc && prevWaypointIndex == waypointIndex && prevFracSegment > fracSegment) {
        reverseMode = !reverseMode;
        reversingProc = true;
        if (reverseMode) {
          waypointIndex--;
        } else {
          waypointIndex++;
        }
      }

      // update position
      transform.position = Vector3.Lerp(prevWaypointPos, nextWaypointPos, fracSegment);

      transform.rotation = Quaternion.Lerp(prevRotation, nextRotation, fracSegment);

      prevFracSegment = fracSegment;
      prevWaypointIndex = waypointIndex;


      rotateWheels(currentSpeed * 50);

      // add graph data
      intervalTime += Time.deltaTime;
      if (intervalTime > graphTimeStep) {
        intervalTime = 0f;
        // add to graphDataTime list
        graphDataTime.Add(intervalTimeCounter * graphTimeStep);
        intervalTimeCounter++;
        // add to graphDataSpeed list
        graphDataEKin.Add(eKin / 1000);
        graphDataEPot.Add(ePot / 1000);
        graphDataETot.Add((eTot + eKinStart) / 1000);
        graphDataAcceleration.Add(currentAcceleration);
        graphDataSpeed.Add(currentSpeed);
        graphDataHeight.Add(nextWaypointPos.y - tracksMinHeight);
      }
    }

    void OnTriggerEnter(Collider collider) {
      // check name to see if it's a waypoint
      if (!collider.CompareTag("Waypoint")) return;

      if (!isMoving) return;

      toNextWaypoint();
    }

    // velocity at the end of the current segment
    private float calculateEndVelocity() {
      return Mathf.Sqrt(eKin * 2 / massCart);
    }

    private float calculateAcceleration() {
      return
        (Mathf.Pow(currentEndVelocity, 2.0f) - Mathf.Pow(currentStartVelocity, 2.0f)) /
        (currentSegmentLength * 2);
    }

    // sets cart to position of first waypoint
    private void setStartPos() {
      gameObject.SetActive(true);
      transform.position = WaypointSystemsList[0].WaypointList[0].WaypointTransform.position;

      if (syncPlayerController) {
        ovrCameraRig.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
      }
    }

    // moves to next waypoint
    private void toNextWaypoint() {

      if (reversingProc) {
        reversingProc = false;
      }

      if (!reverseMode) {
        // switch to next waypointSystem if no more waypoints in current one
        // stop moving if there's no next waypointsystem
        if (waypointIndex + 1 == WaypointSystemsList[waypointSystemsIndex].WaypointList.Count) {
          // check if there's a next waypoint system
          if (waypointSystemsIndex + 1 == WaypointSystemsList.Count) {
            // if not, stop moving
            isMoving = false;
            return;
          } else {
            // if there is, increment index counter
            waypointSystemsIndex++;
            waypointIndex = 0;
          }
        }
      } else {
        // in this case, we're counting down the waypoints because the cart is going backwards
        // switch to next waypointSystem if no more waypoints in current one
        // stop moving if there's no next waypointsystem
        if (waypointIndex == 0) {
          // check if there's a next waypoint system
          if (waypointSystemsIndex == 0) {
            // if not, stop moving
            isMoving = false;
            return;
          } else {
            // if there is, decrement index counter
            waypointSystemsIndex--;
            waypointIndex = WaypointSystemsList[waypointSystemsIndex].WaypointList.Count;
          }
        }
      }

      // get waypoint positions;
      prevWaypointPos = transform.position;

      prevRotation = Quaternion.FromToRotation(transform.forward, nextUnitVectorInter) * transform.rotation;

      if (!reverseMode) {
        nextWaypointPos = WaypointSystemsList[waypointSystemsIndex].WaypointList[waypointIndex + 1].WaypointTransform.position;
        waypointIndex++;

        // get a normalized vector between wayopints

        nextUnitVectorInter = (nextWaypointPos - prevWaypointPos).normalized;
      } else {
        nextWaypointPos = WaypointSystemsList[waypointSystemsIndex].WaypointList[waypointIndex - 1].WaypointTransform.position;
        waypointIndex--;

        // get a normalized vector between wayopints
        nextUnitVectorInter = (prevWaypointPos - nextWaypointPos).normalized;
      }

      // update rotation
      Transform tempTransform = transform;
      tempTransform.rotation = Quaternion.FromToRotation(tempTransform.forward, nextUnitVectorInter) * transform.rotation;
       // handle z rotation
      Vector3 waypointVec = WaypointSystemsList[waypointSystemsIndex].WaypointList[waypointIndex].WaypointVec;;
      Quaternion tiltRotation = Quaternion.FromToRotation(tempTransform.right, waypointVec) * tempTransform.rotation;
      // apply z rotation
      nextRotation = Quaternion.Euler(tempTransform.rotation.eulerAngles.x, tempTransform.rotation.eulerAngles.y, tiltRotation.eulerAngles.z);
      
      // get the segment length to be able to move at correct speed
      currentSegmentLength = Vector3.Distance(prevWaypointPos, nextWaypointPos);

      // make sure it goes off track if track ends prematurely
      if (!trackIsComplete) {
        if (waypointIndex + 2 == WaypointSystemsList[waypointSystemsIndex].WaypointList.Count && waypointSystemsIndex + 1 == WaypointSystemsList.Count - 1)  {
          activatePhysics();
          isMoving = false;
          return;
        }
      }
      

      timeCounter = 0f;
      isMoving = true;

      // calculate end and start velocity and acceleration when switching waypoints

      // calculate potential energy based on height of next waypoint
      ePot = massCart * accelerationGravity * (nextWaypointPos.y - tracksMinHeight);
      // calculate kinetic energy based on difference from total energy
      eKin = eTot - ePot + eKinStart;

      // calculate velocity and acceleration
      // assign last waypoints endvelocity as current start velocity
      currentStartVelocity = currentEndVelocity;
      currentEndVelocity = calculateEndVelocity();
      currentAcceleration = calculateAcceleration();

    }

    private void rotateWheels(float rotationSpeed) {
      Vector3 rotationVector;
      if (!reverseMode) {
        rotationVector = new Vector3(-1, 0, 0);
      } else {
        rotationVector = new Vector3(1, 0, 0);
      }
      wheelsFront.Rotate(rotationVector, rotationSpeed * Time.deltaTime);
      wheelsBack.Rotate(rotationVector, rotationSpeed * Time.deltaTime);
    }

    private void createEmptyGraphs() {
      GraphableDescription eKinGraph = new GraphableDescription("Kinetic Energy", "Kinetic Energy", "Time [s]", "Energy [kJ]");
      GraphableDescription ePotGraph = new GraphableDescription("Potential Energy", "Potential Energy", "Time [s]", "Energy [kJ]");
      GraphableDescription eTotGraph = new GraphableDescription("Total Energy", "Total Energy", "Time [s]", "Energy [kJ]");
      GraphableDescription speedGraph = new GraphableDescription("Speed", "Speed", "Time [s]", "Speed [m/s]");
      GraphableDescription accelerationGraph = new GraphableDescription("Acceleration", "Acceleration", "Time [s]", "Acceleration [m/s^2]");
      GraphableDescription heightGraph = new GraphableDescription("Height", "Height", "Time [s]", "Height [m]");

      graphCreator.CreateEmptyGraph(eKinGraph);
      graphCreator.CreateEmptyGraph(ePotGraph);
      graphCreator.CreateEmptyGraph(eTotGraph);
      graphCreator.CreateEmptyGraph(speedGraph);
      graphCreator.CreateEmptyGraph(accelerationGraph);
      graphCreator.CreateEmptyGraph(heightGraph);
    }

    // activates unity physics (happens when cart goes off tracks)
    private void activatePhysics() {
      fallBoxCollider.enabled = true;
      float thrust = currentEndVelocity * massCart;
      rb.useGravity = true;
      rb.isKinematic = false;
      rb.AddForce(transform.forward * thrust, ForceMode.Impulse);

      physicsActive = true;

      // make sure car resets after x seconds
      StartCoroutine(resetCartCoroutine());
    }

    private IEnumerator resetCartCoroutine() {
      yield return new WaitForSeconds(5.0f);
      rollerCoasterBuilderController.StartStopCart();
    }

    private IEnumerator syncPlayerStopCoroutine() {
      yield return new WaitForSeconds(4.0f);
      SyncPlayerController(false);
    }


    public void ResetCart() {
      // total energy - we use height of first waypoint
      eTot = massCart * accelerationGravity * (WaypointSystemsList[0].WaypointList[0].WaypointTransform.position.y - tracksMinHeight);

      eKin = 0;
      ePot = eTot;

      waypointIndex = 0;
      waypointSystemsIndex = 0;
      isMoving = false;
      isRunning = false;
      physicsActive = false;
      syncPlayerController = false;
      reversingProc = false;
      timeZeroAdded = false;
      // speed float translates to meters per second
      currentSpeed = 0.0f;
      timeCounter = 0f;
      intervalTime = 0f;
      intervalTimeCounter = 1;

      currentEndVelocity = 0f;
      currentStartVelocity = 0f;

      reverseMode = false;

      transform.rotation = Quaternion.Euler(0, 180, 0);
      transform.Rotate(0, -45, 0, Space.World);

      rb.useGravity = false;
      rb.isKinematic = true;

      fallBoxCollider.enabled = false;

      graphDataTime = new List<float>();
      graphDataSpeed = new List<float>();
      graphDataHeight = new List<float>();
      graphDataAcceleration = new List<float>();
      graphDataEKin = new List<float>();
      graphDataEPot = new List<float>();
      graphDataETot = new List<float>();

      setStartPos();

      audioSourceInstantiate.Play();
    }

    public void StartStop() {
      if (isRunning) {
        if (syncPlayerController) {
          StartCoroutine(syncPlayerStopCoroutine());
        }
        graphData();
        ResetCart();
      } else {
        toNextWaypoint();
        isRunning = true;
      }
    }

    private void graphData() {
        graphCreator.ClearGraphs();
        createEmptyGraphs();
        graphCreator.AddToDataSet(new GraphableData(graphDataTime, graphDataEKin, dataName, dataColor), "Kinetic Energy");
        graphCreator.AddToDataSet(new GraphableData(graphDataTime, graphDataEPot, dataName, dataColor), "Potential Energy");
        graphCreator.AddToDataSet(new GraphableData(graphDataTime, graphDataETot, dataName, dataColor), "Total Energy");
        graphCreator.AddToDataSet(new GraphableData(graphDataTime, graphDataSpeed, dataName, dataColor), "Speed");
        graphCreator.AddToDataSet(new GraphableData(graphDataTime, graphDataHeight, dataName, dataColor), "Height");
        graphCreator.AddToDataSet(new GraphableData(graphDataTime, graphDataAcceleration, dataName, dataColor), "Acceleration");
        graphCreator.CreateGraph();
    }

    // take player along for the ride :-)
    public void SyncPlayerController(bool enable) {
      if (enable) {
        // parent camera to cart
        Transform userPosCart = transform.Find("User Position");
        ovrCameraPrevRot = ovrCameraRig.transform.rotation;
        ovrCameraRig.transform.parent = userPosCart;
        audioSourceCountdown.Play();
      } else {
        // parent camera to playercontroller
        ovrCameraRig.transform.parent = playerControllerGO.transform;
        ovrCameraRig.transform.position = playerControllerGO.transform.position;
        ovrCameraRig.transform.rotation = ovrCameraPrevRot;
      }

      syncPlayerController = enable;
      playerController.EnableRay(!enable);
    }

    public void AddGraphReference(GraphCreator _graphCreator) {
      graphCreator = _graphCreator;
    }

    public void TrackIsComplete(bool isComplete) {
      trackIsComplete = isComplete;
    }
  }
}
