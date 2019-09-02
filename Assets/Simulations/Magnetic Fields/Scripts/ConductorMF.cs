using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // handles logic for a conductor with a magnetic field
  public class ConductorMF : MonoBehaviour {

    private float maxMFMagnitude;
    private float initialRadius;
    private float current;

    [SerializeField] private Color strongColor;
    [SerializeField] private Color weakColor;
    [SerializeField] private int nLines = 7;
    [SerializeField] private int nSegments = 30;
    [SerializeField] private int maxCurrent = 10;
    [SerializeField] private float lineWidth = 0.004f;
    [SerializeField] private Material lineMat;
    [SerializeField] private Transform MFLines;

    void Start() {
      current = 1f;
      initialRadius = 0.05f;
      maxMFMagnitude = getMagnitudeMF(initialRadius, maxCurrent);

      displayMF();
    }

    // calculates positions for linepoints
    private void drawPoints(LineRenderer currentLineRenderer, float offset, float horizRadius, float vertRadius) {
      float x = 0f;
      float y;
      float z;

      float angle = 0f;

      Vector3[] linePoints = new Vector3[nSegments + 1];

      for (int i = 0; i < nSegments + 1; i++) {
        y = Mathf.Sin(Mathf.Deg2Rad * angle) * horizRadius;
        z = Mathf.Cos(Mathf.Deg2Rad * angle) * vertRadius;

        linePoints[i] = new Vector3(x, y, z);

        angle += (360f / nSegments);
      }

      currentLineRenderer.positionCount = linePoints.Length;
      currentLineRenderer.SetPositions(linePoints);
    }

    // displays magnetic field lines around conductor
    private void displayMF() {
      for (int i = 1; i <= nLines; i++) {
        GameObject currentGO = new GameObject();
        currentGO.transform.parent = MFLines;
        currentGO.transform.position = MFLines.position;

        // calculate MF magnitude
        float currentRadius = i * initialRadius;
        float currentMagnitudeMF = getMagnitudeMF(currentRadius, current);

        // normalize between 0 and 1 (to be used for color lerp)
        float normalizedMagnitude = currentMagnitudeMF / maxMFMagnitude;

        // get color
        Color currentColor = Color.Lerp(weakColor, strongColor, normalizedMagnitude);

        // create linerenderer and draw points
        LineRenderer currentLineRenderer = createLineRenderer(currentGO, lineWidth, currentColor);
        drawPoints(currentLineRenderer, 0f, currentRadius, currentRadius);
      }      
    }

    // adds a LineRenderer component to a given GameObject and returns the LineRenderer
    private LineRenderer createLineRenderer(GameObject currentGO, float lineWidth, Color currentColor) {
      LineRenderer lineRenderer = currentGO.AddComponent<LineRenderer>() as LineRenderer;
      lineRenderer.useWorldSpace = false;
      lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      lineRenderer.receiveShadows = false;
      lineRenderer.startWidth = lineWidth;
      lineRenderer.endWidth = lineWidth;
      // create material
      // Material newMat = new Material(Shader.Find("Standard"));
      // // set rendering mode to Fade Mode
      // newMat.SetFloat("_Mode", 2);
      // newMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
      // newMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
      // newMat.SetInt("_ZWrite", 0);
      // newMat.DisableKeyword("_ALPHATEST_ON");
      // newMat.EnableKeyword("_ALPHABLEND_ON");
      // newMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
      // newMat.renderQueue = 3000;
      // set its color
      //newMat.color = currentColor;
      lineMat.color = currentColor;
      lineRenderer.material = lineMat;

      return lineRenderer;
    }

    // calculates magnitude of magnetic field at given distance with given current
    private float getMagnitudeMF(float _distance, float _current) {
      float permeabilityOfFreeSpace = 4 * Mathf.PI * Mathf.Pow(10, -7);

      return (permeabilityOfFreeSpace * _current) / (2 * Mathf.PI * _distance);
    }

    public void SetCurrent(float _current) {
      if (current == _current) return;

      // if current has changed, update magnetic field
      current = _current;
      displayMF();
    }
  }
}
