using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // handles logic for a conductor with a magnetic field
  public class ConductorMF : MonoBehaviour {

    private bool isCalculating;
    private bool addNewLineNext;
    private Color currentColor;
    private float maxMFMagnitude;
    private float initialRadius;
    private float otherCurrent;
    private float deltaDistance;
    private int lineCounter;
    private int linePointCounter;
    private LineRenderer[] linesArray;
    private LineRenderer activeLineRendererRight;
    private LineRenderer activeLineRendererLeft;
    private GameObject[] arrowsArray;
    private Vector3 initialPos;
    private Vector3 nextPointRight;
    private Vector3 nextPointLeft;

    public enum CurrentType {ac, dc};
    public CurrentType type;

    [SerializeField] private Color strongColor;
    [SerializeField] private Color weakColor;
    [SerializeField] private int nLines = 7;
    [SerializeField] private int nSegments = 30;
    [SerializeField] private int maxCurrent = 10;
    [SerializeField] private float lineWidth = 0.01f;
    [SerializeField] private float current = 1f;
    [SerializeField] private Material lineMat;
    [SerializeField] private Transform MFLines;
    [SerializeField] private GameObject directionArrowPrefab;
    [SerializeField] private GameObject otherConductor;

    public float Current {
      get { return current; }
    }


    void Start() {
      if (otherConductor) {
        otherCurrent = otherConductor.GetComponent<ConductorMF>().Current;
      } else  {
        otherCurrent = 0;
      }

      addNewLineNext = true;
      deltaDistance = 0.003f;
      //initialRadius = 0.05f;
      initialPos = new Vector3(0, 0, 0.1f);
      //maxMFMagnitude = getMagnitudeMF(initialRadius, maxCurrent);
      maxMFMagnitude = calculateMagneticFieldVector(maxCurrent, 0, initialPos).magnitude;

      //linesArray = new LineRenderer[nLines];
      arrowsArray = new GameObject[nLines];

      displayInitialMF();
    }

    void Update() {

      if (lineCounter < nLines) {
        getNextPoint();
      }
    }

    
    // calculates positions for linepoints
    // private void drawPoints(LineRenderer currentLineRenderer, float offset, float horizRadius, float vertRadius) {
    //   float x = 0f;
    //   float y;
    //   float z;

    //   float angle = 0f;

    //   Vector3[] linePoints = new Vector3[nSegments + 1];

    //   for (int i = 0; i < nSegments + 1; i++) {
    //     y = Mathf.Sin(Mathf.Deg2Rad * angle) * horizRadius;
    //     z = Mathf.Cos(Mathf.Deg2Rad * angle) * vertRadius;

    //     linePoints[i] = new Vector3(x, y, z);

    //     angle += (360f / nSegments);
    //   }

    //   currentLineRenderer.positionCount = linePoints.Length;
    //   currentLineRenderer.SetPositions(linePoints);
    // }

     private void drawPoints(LineRenderer currentLineRenderer, List<Vector3> _linePoints) {
     
      currentLineRenderer.positionCount = _linePoints.Count;
      currentLineRenderer.SetPositions(_linePoints.ToArray());
    }


    // *initially* displays magnetic field lines around conductor
    // private void displayInitialMF() {
      // for (int i = 1; i <= nLines; i++) {
      //   GameObject currentGO = new GameObject();
      //   currentGO.transform.parent = MFLines;
      //   currentGO.transform.position = MFLines.position;

      //   // calculate MF magnitude
      //   float currentRadius = i * initialRadius;
      //   float currentMagnitudeMF = getMagnitudeMF(currentRadius, current);

      //   Color currentColor = calculateColor(currentMagnitudeMF);

      //   // create linerenderer and draw points
      //   LineRenderer currentLineRenderer = createLineRenderer(currentGO, lineWidth, currentColor);

      //   // add to array
      //   linesArray[i - 1] = currentLineRenderer;
      //   drawPoints(currentLineRenderer, 0f, currentRadius, currentRadius);

      //   // add direction arrows
      //   GameObject currentArrow = createDirectionArrows(currentRadius, currentRadius, currentColor);
      //   arrowsArray[i - 1] = currentArrow;
      // }      
    // }

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
        Vector3 initialMF = calculateMagneticFieldVector(current, otherCurrent, nextPointRight);
        currentColor = calculateColor(initialMF.magnitude);

        activeLineRendererRight = createLineRenderer(lineGameObjectRight, lineWidth, currentColor);
        activeLineRendererLeft = createLineRenderer(lineGameObjectLeft, lineWidth, currentColor);

        // add first points to make sure they are at the same spot
        activeLineRendererRight.positionCount = 1;
        activeLineRendererLeft.positionCount = 1;
        activeLineRendererRight.SetPosition(0, nextPointRight);
        activeLineRendererLeft.SetPosition(0, nextPointLeft);

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
        setArrowAttributes(currentArrowRight, currentColor);
        setArrowAttributes(currentArrowLeft, currentColor);

        // reset values
        addNewLineNext = true;
        lineCounter++;
        linePointCounter = 1;
        return;
      }

    }

    private void displayInitialMF() {
      lineCounter = 0;
      linePointCounter = 1;   
    }

    // private void displayInitialMF() {
    //   for (int i = 0; i < nLines; i++) {
    //     GameObject currentGO = new GameObject();
    //     currentGO.transform.parent = MFLines;
    //     currentGO.transform.position = MFLines.position;

    //     Vector3 startPos = initialPos + (i * new Vector3(0, 0, 0.05f));

    //     // calculate mf vector for initial point
    //     Vector3 initialMF = calculateMagneticFieldVector(current, startPos);

    //     List<Vector3> linePoints = new List<Vector3>();

    //     Vector3 prevPoint = startPos;
    //     linePoints.Add(prevPoint);

    //     bool reachedStartPos = false;
    //     int counter = 1;

    //     while (!reachedStartPos && counter < 300 * (i + 1)) {
    //       // get next point with RK4
    //       Vector3 nextPoint = getNextMFPointRK4(current, prevPoint, deltaDistance);

    //       // add to array
    //       if (counter % 20 == 0) {
    //         linePoints.Add(nextPoint);
    //       }

    //       prevPoint = nextPoint;

    //       // check if close to start position
    //       if (counter > 100 && Vector3.Distance(startPos, nextPoint) < 0.004f) {
    //         reachedStartPos = true;
    //       }

    //       counter += 1;
    //     }

    //     Color currentColor = calculateColor(initialMF.magnitude);

    //     // create linerenderer and draw points
    //     LineRenderer currentLineRenderer = createLineRenderer(currentGO, lineWidth, currentColor);

    //     // add to array
    //     linesArray[i] = currentLineRenderer;
    //     drawPoints(currentLineRenderer, linePoints);

    //     // add direction arrows
    //     // GameObject currentArrow = createDirectionArrows(currentRadius, currentRadius, currentColor);
    //     // arrowsArray[i - 1] = currentArrow;
    //   }    
    // }

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

    // "*updates* magnetic field lines with new magnetic field strength
    // private void updateMF() {
    //   for (int i = 0; i < nLines; i++) {
    //     // calculate MF magnitude
    //     float currentRadius = (i + 1) * initialRadius;
    //     float currentMagnitudeMF = getMagnitudeMF(currentRadius, current);

    //     Color currentColor = calculateColor(currentMagnitudeMF);
    //     setLineAttribtues(linesArray[i], lineWidth, currentColor);
    //     setArrowAttributes(arrowsArray[i], currentColor);
    //   }
    // }

    private Color calculateColor(float _currentMagnitudeMF) {
      // normalize between 0 and 1 (to be used for color lerp)
      float normalizedMagnitude = _currentMagnitudeMF / maxMFMagnitude;
      // get color
      Color currentColor = Color.Lerp(weakColor, strongColor, normalizedMagnitude);

      return currentColor;
    }

    private GameObject createDirectionArrows(Vector3 randomPos, Vector3 randomPosNext, string _direction) {
      GameObject directionArrow = (GameObject)Instantiate(directionArrowPrefab, new Vector3(0, 0, 0), Quaternion.identity);

      directionArrow.transform.parent = transform;

      float newAngle = Vector3.Angle(randomPos, randomPosNext);

      directionArrow.transform.localPosition = randomPos;
      directionArrow.transform.rotation = Quaternion.Euler(newAngle, 0, 0);

      // rotate by another 180 degrees if the current direction is forward
      if (current > 0 && _direction == "right") {
        directionArrow.transform.Rotate(180, 0, 0);
      }

      // rotate another 180 if direction is left
      // if (_direction == "left") {
      //   directionArrow.transform.Rotate(180, 0, 0);
      // }

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

    // calculates magnitude of magnetic field at given distance with given current
    // private float getMagnitudeMF(float _distance, float _current) {
    //   float permeabilityOfFreeSpace = 4 * Mathf.PI * Mathf.Pow(10, -7);

    //   return (permeabilityOfFreeSpace * _current) / (2 * Mathf.PI * _distance);
    // }

    public void SetCurrent(float _current) {
      if (current == _current) return;

      // if current has changed, update magnetic field
      current = _current;
      //updateMF();
    }
  }
}
