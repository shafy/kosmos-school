using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // handles logic for a conductor with a magnetic field
  public class ConductorMF : MonoBehaviour {

    private float lineWidth;
    private float horizRadius;
    private float vertRadius;
    private int nSegments;

    [SerializeField] private Transform MFLines;
    [SerializeField] private Material lineMaterial;

    private int current;

    void Start() {
      lineWidth = 0.004f;
      nSegments = 30;
      current = 5;

      displayMF();
    }

    void Update() {
      //displayMF();
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

      // we define min and max radii
      // depending on the ampere, there are more or less field lines
      // denser lines == stronger magnetic field

      float minHorizRadius = 0.015f;
      float minVertRadius = 0.015f;

      float maxHorizRadius = 0.25f;
      float maxVertRadius = 0.25f; 

      // so that we have at least 3 lines
      //int nLines = current + 2;
      int nLines = 7;

      float initialRadius = 0.035f;

      // we get the initial radius, because the radius grows squared
      //float initialRadius = (maxHorizRadius - minHorizRadius) / Mathf.Pow(nLines, 2);

      for (int i = 1; i <= nLines; i++) {
        GameObject currentGO = new GameObject();
        currentGO.transform.parent = MFLines;
        currentGO.transform.position = MFLines.position;
        LineRenderer currentLineRenderer = createLineRenderer(currentGO, lineWidth);
        //drawPoints(currentLineRenderer, 0f, Mathf.Pow(1.5f, i) * initialRadius, Mathf.Pow(1.5f, i) * initialRadius);
        drawPoints(currentLineRenderer, 0f, i * initialRadius, i * initialRadius);
      }      
    }

    // adds a LineRenderer component to a given GameObject and returns the LineRenderer
    private LineRenderer createLineRenderer(GameObject currentGO, float lineWidth) {
      LineRenderer lineRenderer = currentGO.AddComponent<LineRenderer>() as LineRenderer;
      lineRenderer.useWorldSpace = false;
      lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      lineRenderer.receiveShadows = false;
      lineRenderer.startWidth = lineWidth;
      lineRenderer.endWidth = lineWidth;
      lineRenderer.material = lineMaterial;

      return lineRenderer;
    }
  }
}
