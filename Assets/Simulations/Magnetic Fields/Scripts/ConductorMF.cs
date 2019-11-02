using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // handles logic for a conductor with a magnetic field
  public class ConductorMF : MonoBehaviour {

    private bool isCalculating;
    private bool addNewLineNext;
    private bool createMF;
    private float maxMFMagnitude;
    private float initialRadius;
    private float otherCurrent;
    private float deltaDistance;
    private float current;
    private float acCycleTime;
    private float prevIntervaltime;
    private float prevPrevIntervaltime;
    private int lineCounter;
    private int linePointCounter;
    private LineRenderer[] linesArray;
    private LineRenderer activeLineRendererRight;
    private LineRenderer activeLineRendererLeft;
    private List<float> mfQueue;
    private List<GameObject> linesListRight;
    private List<GameObject> linesListLeft;
    private List<Color> initialColors;
    private List<float> initialLineWidths;
    private List<GameObject> arrowsListRight;
    private List<GameObject> arrowsListLeft;
    private Vector3 initialPos;
    private Vector3 initialMF;
    private Vector3 nextPointRight;
    private Vector3 nextPointLeft;

    public enum CurrentType {ac, dc};
    public CurrentType currentType;

    [SerializeField] private Color strongColor;
    [SerializeField] private Color weakColor;
    [SerializeField] private Color strongArrowColor;
    [SerializeField] private Color weakArrowColor;
    [SerializeField] private int nLines = 7;
    [SerializeField] private int nSegments = 30;
    [SerializeField] private int maxCurrent = 10;
    [SerializeField] private float maxLineWidth = 0.03f;
    [SerializeField] private float minLineWidth = 0.005f;
    [SerializeField] private Material lineMat;
    [SerializeField] private Transform MFLines;
    [SerializeField] private GameObject directionArrowPrefab;
    [SerializeField] private GameObject otherConductor;

    public float Current {
      get { return current; }
    }

    public bool InProgress {
      get { return createMF; }
    }

    void Start() {
      current = 0;
      if (otherConductor) {
        otherCurrent = otherConductor.GetComponent<ConductorMF>().Current;
      } else  {
        otherCurrent = 0;
      }

      addNewLineNext = true;
      deltaDistance = 0.003f;
      initialPos = new Vector3(0, 0, 0.1f);
      maxMFMagnitude = calculateMagneticFieldVector(maxCurrent, 0, initialPos).magnitude;

      createMF = false;

      mfQueue = new List<float>();

      // one AC cycle takes x seconds
      acCycleTime = 0.5f;
    }

    void Update() {

      // keep changing colors to indicate AC
      if (currentType == CurrentType.ac && !createMF && Mathf.Abs(current) > 0) {
        lerpLineColorsAndWidth();
      }

      if (!createMF) {
        // check queue
        if (mfQueue.Count == 0) return;
        startNextInQueue();
        return;
      }

      if (lineCounter < nLines) {
        getNextPoint();
      } else {
        // stop drawing magnetic field
        createMF = false;
      }

    }

    // lerps through range of colors for the line (used for AC)
    private void lerpLineColorsAndWidth() {
      float intervalTime = Mathf.PingPong(Time.time / acCycleTime, 1);

      for (int i = 0; i < linesListRight.Count; i++) {

        // get color
        Color newColor = Color.Lerp(weakColor, initialColors[i], intervalTime);

        // get width
        float newLineWidth = Mathf.Lerp(minLineWidth, initialLineWidths[i], intervalTime);

        LineRenderer currentRendererRight = linesListRight[i].GetComponent<LineRenderer>();
        LineRenderer currentRendererLeft = linesListLeft[i].GetComponent<LineRenderer>();
    
        currentRendererRight.material.color = newColor;
        currentRendererLeft.material.color = newColor;

        currentRendererRight.startWidth = newLineWidth;
        currentRendererRight.endWidth = newLineWidth;
        currentRendererLeft.startWidth = newLineWidth;
        currentRendererLeft.endWidth = newLineWidth;

        // flip arrows
        if ((intervalTime > prevIntervaltime && prevIntervaltime < prevPrevIntervaltime) || (intervalTime < prevIntervaltime && prevIntervaltime > prevPrevIntervaltime)) {
          arrowsListRight[i].transform.Rotate(180, 0, 0);
          arrowsListLeft[i].transform.Rotate(180, 0, 0);
        }
      }

      prevPrevIntervaltime = prevIntervaltime;
      prevIntervaltime = intervalTime;
    }

    // starts next MF in queue
    private void startNextInQueue() {
      current = mfQueue[0];
      mfQueue.RemoveAt(0);
      displayMF();
    }

    // calculates the next point on the magnetic field line
    private void getNextPoint() {
      // in this case we have to add a new LineRenderer
      if (addNewLineNext) {
        GameObject lineGameObjectRight = new GameObject();
        lineGameObjectRight.transform.parent = MFLines;
        lineGameObjectRight.transform.position = MFLines.position;

        GameObject lineGameObjectLeft = new GameObject();
        lineGameObjectLeft.transform.parent = MFLines;
        lineGameObjectLeft.transform.position = MFLines.position;

        nextPointRight = initialPos + (lineCounter * new Vector3(0, 0, 0.1f));
        nextPointLeft = nextPointRight; 

        // calculate mf vector for initial point
        initialMF = calculateMagneticFieldVector(current, otherCurrent, nextPointRight);
        Color currentColor = calculateColor(initialMF.magnitude, weakColor, strongColor);
        float currentLineWidth = calculateWidth(initialMF.magnitude);

        activeLineRendererRight = createLineRenderer(lineGameObjectRight, currentLineWidth, currentColor);
        activeLineRendererLeft = createLineRenderer(lineGameObjectLeft, currentLineWidth, currentColor);

        // add first points to make sure they are at the same spot
        activeLineRendererRight.positionCount = 1;
        activeLineRendererLeft.positionCount = 1;
        activeLineRendererRight.SetPosition(0, nextPointRight);
        activeLineRendererLeft.SetPosition(0, nextPointLeft);

        // add to list of lines and initial colors
        linesListRight.Add(lineGameObjectRight);
        linesListLeft.Add(lineGameObjectLeft);
        initialColors.Add(currentColor);
        initialLineWidths.Add(currentLineWidth);

        addNewLineNext = false;
      }
      
      bool reachedStartPos = false;

      // get 20 next points, then add it to the linePoints array (we draw this one)
      for (int i = 0; i < 20; i++) {
        nextPointRight = getNextMFPointRK4(nextPointRight, deltaDistance);
        nextPointLeft = getNextMFPointRK4(nextPointLeft, -deltaDistance);

        // if close to start point, stop executing
        if (linePointCounter > 3 && Vector3.Distance(nextPointRight, nextPointLeft) < 0.004f) {
          reachedStartPos = true;
          break;
        }
      }

      activeLineRendererRight.positionCount = linePointCounter + 1;
      activeLineRendererLeft.positionCount = linePointCounter + 1;
      activeLineRendererRight.SetPosition(linePointCounter, nextPointRight);
      activeLineRendererLeft.SetPosition(linePointCounter, nextPointLeft);
      //linePoints.Add(prevPoint);

      linePointCounter++;

      // after x calculated points, increase lineCounter and reset linePointCounter
      if (linePointCounter == 300 || reachedStartPos ) {
        // add direction arrow
        int arrowPosIndex = (activeLineRendererRight.positionCount - 1) / 2;

        Vector3 arrowPosRight = activeLineRendererRight.GetPosition(arrowPosIndex);
        Vector3 arrowPosRightNext = activeLineRendererRight.GetPosition(arrowPosIndex + 1);
        Vector3 arrowPosLeft = activeLineRendererLeft.GetPosition(arrowPosIndex);
        Vector3 arrowPosLeftNext = activeLineRendererLeft.GetPosition(arrowPosIndex - 1);
       
        GameObject currentArrowRight = createDirectionArrows(arrowPosRight, arrowPosRightNext, "right");
        GameObject currentArrowLeft = createDirectionArrows(arrowPosLeft, arrowPosLeftNext, "left");

        Color currentArrowColor = calculateColor(initialMF.magnitude, weakArrowColor, strongArrowColor);
        setArrowAttributes(currentArrowRight, currentArrowColor);
        setArrowAttributes(currentArrowLeft, currentArrowColor);

        // add arrows to list
        arrowsListRight.Add(currentArrowRight);
        arrowsListLeft.Add(currentArrowLeft);

        // reset values
        addNewLineNext = true;
        lineCounter++;
        linePointCounter = 1;
        return;
      }
    }

    private void displayMF() {

      // if already creation in progress add to queue
      if (createMF) {
        mfQueue.Add(current);
        return;
      }

      destroyMF();

      // don't draw a new one if current is 0
      if (current == 0) return;

      createMF = true;
      lineCounter = 0;
      linePointCounter = 1;
      linesListRight = new List<GameObject>();
      linesListLeft = new List<GameObject>();
      initialColors = new List<Color>();
      initialLineWidths = new List<float>();
      arrowsListRight = new List<GameObject>();
      arrowsListLeft = new List<GameObject>();
    }

    // destroys magnetic field if it exists
    private void destroyMF() {
      foreach (Transform child in MFLines) {
        Destroy(child.gameObject);
      }
    }

    private Vector3 calculateMagneticFieldVector(float _current, float _otherCurrent, Vector3 _distanceVec) {
      float permeabilityOfFreeSpace = 4 * Mathf.PI * Mathf.Pow(10, -7);

      // current vector goes along the cable
      Vector3 currentVector = new Vector3(_current, 0, 0);
      Vector3 currentVectorOther = new Vector3(_otherCurrent, 0, 0);
      
      // distance/radius defined on y/z plane
      // return in micro tesla

      Vector3 MFVectorThisConductor = 1000000 * (permeabilityOfFreeSpace * Vector3.Cross(currentVector, _distanceVec.normalized) / (4 * Mathf.PI * Mathf.Pow(_distanceVec.magnitude, 2)));

      // calculate MF of other conductor
      Vector3 distanceVecOther;
      Vector3 MFVectorOtherConductor = new Vector3(0, 0, 0);

      if (otherConductor) {
        distanceVecOther = _distanceVec + (transform.position - otherConductor.transform.position);
        MFVectorOtherConductor = 1000000 * (permeabilityOfFreeSpace * Vector3.Cross(currentVectorOther, distanceVecOther.normalized) / (4 * Mathf.PI * Mathf.Pow(distanceVecOther.magnitude, 2)));
      }
     
      return MFVectorThisConductor + MFVectorOtherConductor;
    }

    // returns the next point of the magnetic field line that has the same magnitude
    // algorithm based on Runge-Kutta 4 (https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods)
    private Vector3 getNextMFPointRK4(Vector3 distanceVec, float _deltaDistance) {
      // _deltaDistance is the step size

      // we normalize the vectors because RK4 shouldn't be dependent on strenght of magnetic field
      Vector3 k1Vector = _deltaDistance * calculateMagneticFieldVector(current, otherCurrent, distanceVec).normalized;
      Vector3 k2Vector = _deltaDistance * calculateMagneticFieldVector(current, otherCurrent, distanceVec + (k1Vector * (_deltaDistance / 2))).normalized;
      Vector3 k3Vector = _deltaDistance * calculateMagneticFieldVector(current, otherCurrent, distanceVec + (k2Vector * (_deltaDistance / 2))).normalized;
      Vector3 k4Vector = _deltaDistance * calculateMagneticFieldVector(current, otherCurrent, distanceVec + (k3Vector * _deltaDistance)).normalized;

      // return next point
      return distanceVec + (k1Vector + 2 * k2Vector + 2 * k3Vector + k4Vector) / 6;
    }

    private Color calculateColor(float _currentMagnitudeMF, Color _weakColor, Color _strongColor) {
      // normalize between 0 and 1 (to be used for color lerp)
      float normalizedMagnitude = _currentMagnitudeMF / maxMFMagnitude;
      // get color
      Color currentColor = Color.Lerp(_weakColor, _strongColor, normalizedMagnitude);

      return currentColor;
    }

    private float calculateWidth(float _currentMagnitudeMF) {
       // normalize between 0 and 1 (to be used for color lerp)
      float normalizedMagnitude = _currentMagnitudeMF / maxMFMagnitude;
      float newLineWidth = Mathf.Lerp(minLineWidth, maxLineWidth, normalizedMagnitude);

      return newLineWidth;
    }

    private GameObject createDirectionArrows(Vector3 randomPos, Vector3 randomPosNext, string _direction) {
      GameObject directionArrow = (GameObject)Instantiate(directionArrowPrefab, new Vector3(0, 0, 0), Quaternion.identity);

      directionArrow.transform.parent = MFLines.transform;

      float newAngle = Vector3.Angle(randomPos, randomPosNext);

      directionArrow.transform.localPosition = randomPos;
      directionArrow.transform.rotation = Quaternion.Euler(newAngle, 0, 0);

      if (_direction == "right") {
        directionArrow.transform.Rotate(180, 0, 0);
      }

      return directionArrow;
    }


    private LineRenderer setLineAttribtues(LineRenderer _lineRenderer, float lineWidth, Color currentColor) {
      _lineRenderer.startWidth = lineWidth;
      _lineRenderer.endWidth = lineWidth;

      // create new material
      Material newMat = new Material(Shader.Find("Standard"));
      newMat.CopyPropertiesFromMaterial(lineMat);
      newMat.color = currentColor;
      _lineRenderer.material = newMat;

      return _lineRenderer;
    }

    private GameObject setArrowAttributes(GameObject _directionArrow, Color currentColor) {
      // create new material
      Material newMat = new Material(Shader.Find("Standard"));
      newMat.CopyPropertiesFromMaterial(lineMat);
      newMat.color = currentColor;
      _directionArrow.transform.GetChild(0).GetComponent<Renderer>().material = newMat;

      return _directionArrow;
    }

    // adds a LineRenderer component to a given GameObject and returns the LineRenderer
    private LineRenderer createLineRenderer(GameObject currentGO, float lineWidth, Color currentColor) {
      LineRenderer lineRenderer = currentGO.AddComponent<LineRenderer>() as LineRenderer;
      lineRenderer.useWorldSpace = false;
      lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      lineRenderer.receiveShadows = false;
      lineRenderer.loop = false; // connect end and start point together
      lineRenderer = setLineAttribtues(lineRenderer, lineWidth, currentColor);

      return lineRenderer;
    }

    public void SetCurrent(float _current) {
      if (current == _current) return;

      // if current has changed, update magnetic field
      current = _current;

      displayMF();
    }

    public void SetOtherCurrent(float _otherCurrent) {
      otherCurrent = _otherCurrent;
    }

    public void SetOtherConductor(GameObject _otherConductor) {
      otherConductor = _otherConductor;
    }

    public void SetCurrentType(CurrentType _currentType) {
      currentType = _currentType;
    }
  }
}
