using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // handles logic for a conductor with a magnetic field
  public class ConductorMF : MonoBehaviour {

    private float maxMFMagnitude;
    private float initialRadius;
    private float current;
    private float deltaDistance;
    private LineRenderer[] linesArray;
    private GameObject[] arrowsArray;
    private Vector3 initialPos;

    private enum CurrentDirection {forward, backward};
    private CurrentDirection direction;

    private enum CurrentType {ac, dc};
    private CurrentType type;

    [SerializeField] private Color strongColor;
    [SerializeField] private Color weakColor;
    [SerializeField] private int nLines = 7;
    [SerializeField] private int nSegments = 30;
    [SerializeField] private int maxCurrent = 10;
    [SerializeField] private float lineWidth = 0.004f;
    [SerializeField] private Material lineMat;
    [SerializeField] private Transform MFLines;
    [SerializeField] private GameObject directionArrowPrefab;

    void Start() {
      current = 1f;
      deltaDistance = 0.0008f;
      //initialRadius = 0.05f;
      initialPos = new Vector3(0, 0, 0.05f);
      //maxMFMagnitude = getMagnitudeMF(initialRadius, maxCurrent);
      maxMFMagnitude = calculateMagneticFieldVector(maxCurrent, initialPos).magnitude;

      linesArray = new LineRenderer[nLines];
      arrowsArray = new GameObject[nLines];

      displayInitialMF();
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

    private void displayInitialMF() {
      for (int i = 1; i <= nLines; i++) {
        GameObject currentGO = new GameObject();
        currentGO.transform.parent = MFLines;
        currentGO.transform.position = MFLines.position;

        // calculate mf vector for initial point
        Vector3 initialMF = calculateMagneticFieldVector(current, initialPos);


        //Vector3[] linePoints = new Vector3[nSegments];
        List<Vector3> linePoints = new List<Vector3>();

        Vector3 prevPoint = initialPos;
        linePoints.Add(prevPoint);

        // for (int y = 1; y < nSegments; y++) {

        //     // get next point with RK4
        //     Vector3 nextPoint = getNextMFPointRK4(current, prevPoint, deltaDistance);

        //     // add to array
        //     linePoints[y] = nextPoint;
        //     prevPoint = nextPoint;
        // }

        bool reachedStartPos = false;
        int counter = 1;

        while (!reachedStartPos && counter < 600) {
          // get next point with RK4
          Vector3 nextPoint = getNextMFPointRK4(current, prevPoint, deltaDistance);

          // add to array
          if (counter % 20 == 0) {
            linePoints.Add(nextPoint);
          }

          prevPoint = nextPoint;

          // check if close to start position
          if (counter > 100 && Vector3.Distance(initialPos, nextPoint) < 0.004f) {
            reachedStartPos = true;
          }

          counter += 1;

        }


        Color currentColor = calculateColor(initialMF.magnitude);

        // create linerenderer and draw points
        LineRenderer currentLineRenderer = createLineRenderer(currentGO, lineWidth, currentColor);

        // add to array
        linesArray[i - 1] = currentLineRenderer;
        drawPoints(currentLineRenderer, linePoints);

        // add direction arrows
        // GameObject currentArrow = createDirectionArrows(currentRadius, currentRadius, currentColor);
        // arrowsArray[i - 1] = currentArrow;
      }    

     

    }

    private Vector3 calculateMagneticFieldVector(float _current, Vector3 _distanceVec) {
      float permeabilityOfFreeSpace = 4 * Mathf.PI * Mathf.Pow(10, -7);

      // current vector goes along the cable
      Vector3 currentVector = new Vector3(_current, 0, 0);

      // distance/radius defined on y/z plane
      // return in micro tesla
      //return 1000000 * ((permeabilityOfFreeSpace * currentVector) / (2 * Mathf.PI * _distanceVec.magnitude));
      return 1000000 * ((permeabilityOfFreeSpace / 4 * Mathf.PI) * (Vector3.Cross(currentVector, _distanceVec) / Mathf.Pow(currentVector.magnitude, 2)));
    }

    // returns the next point of the magnetic field line that has the same magnitude
    // algorithm based on Runge-Kutta 4 (https://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_methods)
    private Vector3 getNextMFPointRK4(float _current, Vector3 distanceVec, float _deltaDistance) {
      // _deltaDistance is the step size

     

      // we normalize the vectors because RK4 shouldn't be dependent on strenght of magnetic field
      Vector3 k1Vector = _deltaDistance * calculateMagneticFieldVector(_current, distanceVec).normalized;
      Vector3 k2Vector = _deltaDistance * calculateMagneticFieldVector(_current, distanceVec + (k1Vector * (_deltaDistance / 2))).normalized;
      Vector3 k3Vector = _deltaDistance * calculateMagneticFieldVector(_current, distanceVec + (k2Vector * (_deltaDistance / 2))).normalized;
      Vector3 k4Vector = _deltaDistance * calculateMagneticFieldVector(_current, distanceVec + (k3Vector * _deltaDistance)).normalized;

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

    private GameObject createDirectionArrows(float horizRadius, float vertRadius, Color _color) {
      GameObject directionArrow = (GameObject)Instantiate(directionArrowPrefab, new Vector3(0, 0, 0), Quaternion.identity);

      directionArrow.transform.parent = transform;

      // define position
      // random position ellipse/circle
      float x = 0f;
      float randomAngle = Random.Range(0, 360);
      float y = Mathf.Sin(Mathf.Deg2Rad * randomAngle) * horizRadius;
      float z = Mathf.Cos(Mathf.Deg2Rad * randomAngle) * vertRadius;

      directionArrow.transform.localPosition = new Vector3(x, y, z);
      directionArrow.transform.Rotate(-randomAngle, 0, 0);

      // rotate by another 180 degrees if the current direction is backward
      if (direction == CurrentDirection.backward) {
        directionArrow.transform.Rotate(180, 0, 0);
      }

      directionArrow = setArrowAttributes(directionArrow, _color);

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
      Debug.Log("_directionArrow.transform.GetChild(0).GetComponent<Renderer>() " + _directionArrow.transform.GetChild(0).GetComponent<Renderer>());
      _directionArrow.transform.GetChild(0).GetComponent<Renderer>().material = newMat;

      return _directionArrow;
    }

    // adds a LineRenderer component to a given GameObject and returns the LineRenderer
    private LineRenderer createLineRenderer(GameObject currentGO, float lineWidth, Color currentColor) {
      LineRenderer lineRenderer = currentGO.AddComponent<LineRenderer>() as LineRenderer;
      lineRenderer.useWorldSpace = false;
      lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      lineRenderer.receiveShadows = false;
      lineRenderer.loop = true; // connect end and start point together
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
