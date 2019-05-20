using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Kosmos
{
  // Creates a graph
  public class Grapher : MonoBehaviour {

    private Color[] colorsAxis;
    private Color[] colorsGrid;
    private int blockWidthLine;
    private int blockHeightLine;
    private int blockWidthGrid;
    private int blockHeightGrid;
    private int border;
    private int lengthX;
    private int lengthY;
    private int nGridLines;
    private float[] xAxisLabelValues;
    private float[] yAxisLabelValues;
    private List<float> predefinedAxisLabels;
    private string xAxisTitle;
    private string yAxisTitle;
    private Texture2D texture;
    private Renderer renderer;

    [SerializeField] private GameObject AxisLabelPrefab;
    [SerializeField] private Transform AxisLabelParentTransform;
    [SerializeField] private TextMeshPro xAxisTMP;
    [SerializeField] private TextMeshPro yAxisTMP;
    

    void Awake() {
      renderer = GetComponent<Renderer>();

      // future to-do: move more params that are hard coded here to GraphLine's arguments for more flexibility

      // create new texture
      texture = new Texture2D(1024, 512);
      renderer.material.mainTexture = texture;

      // define colors array to be used with SetPixels block overload
      // the color is a flattened 2D array that has the length blockWidth * blockHeight
      blockWidthGrid = 2;
      blockHeightGrid = 2;

      // define the number of gridlines per axis
      nGridLines = 5;
      // color light gray
      colorsGrid = Enumerable.Repeat<Color>(new Color(0.9f, 0.9f, 0.9f), blockWidthGrid * blockHeightGrid).ToArray<Color>();

      // define border size and lengths of axes
      border = 64;
      // get lengths
      lengthX = texture.width - 2 * border;
      lengthY = texture.height - 2 * border;
      // make sure it's divisble by number of gridlines
      lengthX = lengthX - (lengthX % nGridLines);
      lengthY = lengthY - (lengthY % nGridLines);

      // these are predefined grid stepss
      predefinedAxisLabels = new List<float> {0.1f, 0.2f, 0.5f, 1f, 2f, 10f, 20f, 50f, 100f, 200f, 500f, 1000f};

    }

    // draws a block of pixels (don't forget to call texture.Apply())
    // make sure that blockWidth * blockHeight = colors.Length
    private void drawPixelBlock(int x, int y, int blockWidth, int blockHeight, Color[] colors) {
      texture.SetPixels(x, y, blockWidth, blockHeight, colors, 0);
    }

    private void setAxesTitles() {
      xAxisTMP.text = xAxisTitle;
      yAxisTMP.text = yAxisTitle;
    }

    // draw x or y axis for graph
    private void drawAxis(string axis, Color[] colors) {
      if (axis == "x") {
        // draw line
        // draw pixel blocks, make sure to increase i by blockWidth
        for (int i = 0; i < lengthX; i += blockWidthLine) {
          drawPixelBlock(i + border, border, blockWidthLine, blockHeightLine, colors);
        }
        // add one more pixel block but starting blockWidthGrid earlier so that the grid isn't
        // wider than the axis
        drawPixelBlock(lengthX + border - blockWidthGrid, border, blockWidthLine, blockHeightLine, colors);

        // draw arrow head
        // get last point using modus
        //int lastPoint = border + lengthX - (lengthX % blockWidthLine);
        //drawArrowHead("x", lastPoint, border);
      } else {
          // draw line
          // draw pixel blocks, make sure to increase i by blockHeight
          for (int i = 0; i < lengthY; i += blockHeightLine) {
            drawPixelBlock(border, i + border, blockWidthLine, blockHeightLine, colors);
          }

          // add one more pixel block but starting blockHeightGrid earlier so that the grid isn't
          // higher than the axis
          drawPixelBlock(border, lengthY + border - blockHeightGrid, blockWidthLine, blockHeightLine, colors);

          // draw arrow head
          // get last point using modus
          //int lastPoint = border + lengthY - (lengthY % blockHeightLine);
          //drawArrowHead("y", border, lastPoint);
      }

      // upload texture to graphics card
      texture.Apply();
    }

    // draws arrow head for x or y axis
    // private void drawArrowHead(string axis, int startingPointX, int startingPointY) {
    //   if (axis == "x") {
    //     // upper part
    //     drawPixelBlock(startingPointX - (2 * blockWidthLine), startingPointY + blockHeightLine, blockWidthLine, blockHeightLine, colorsAxis);
    //     drawPixelBlock(startingPointX - (3 * blockWidthLine), startingPointY + (2 * blockHeightLine), blockWidthLine, blockHeightLine, colorsAxis);

    //     // lower part
    //     drawPixelBlock(startingPointX - (2 * blockWidthLine), startingPointY - blockHeightLine, blockWidthLine, blockHeightLine, colorsAxis);
    //     drawPixelBlock(startingPointX - (3 * blockWidthLine), startingPointY - (2 * blockHeightLine), blockWidthLine, blockHeightLine, colorsAxis);
    //   } else {
    //     // right part
    //     drawPixelBlock(startingPointX + blockWidthLine, startingPointY - (2 * blockHeightLine), blockWidthLine, blockHeightLine, colorsAxis);
    //     drawPixelBlock(startingPointX + (2 * blockWidthLine), startingPointY - (3 * blockHeightLine), blockWidthLine, blockHeightLine, colorsAxis);

    //     // left part
    //     drawPixelBlock(startingPointX - blockWidthLine, startingPointY - (2 * blockHeightLine), blockWidthLine, blockHeightLine, colorsAxis);
    //     drawPixelBlock(startingPointX - (2 * blockWidthLine), startingPointY - (3 * blockHeightLine), blockWidthLine, blockHeightLine, colorsAxis);

    //     }
    // }

    // draws grid with nGridLines lines per axis
    private void drawGrid() {
      // define vars
      float relativeXPos;
      float relativeYPos;
      float quadXPos;
      float quadYPos;

      // x axis grid (horizontal)

      // get 1/nGridLines of lengthY, use modulo to make sure we don't get floats
      // this is not really necessary as the length should be already perfectly
      // divisble from our Start() function
      int xGridHeight = (lengthY - (lengthY % nGridLines)) / nGridLines;
      // draw grid lines
      for (int i = 0; i <= nGridLines; i++) {
        int yPos = border + i * xGridHeight;
        // length should be as long as x axis, make sure to increment by blockWidth
        // don't draw gridline for the first one
        if (i != 0) {
          for (int y = blockWidthLine; y <= lengthX; y += blockWidthGrid) {
            drawPixelBlock(border + y, yPos, blockWidthGrid, blockHeightGrid, colorsGrid);
          }
        }

        // set label
        // translate texture pixel to world space (Vector3)
        relativeYPos = (float)yPos / (float)texture.height;
        quadYPos = transform.localScale.y * relativeYPos;

        relativeXPos = (float)(border - 4 * blockWidthLine) / (float)texture.width;
        quadXPos = transform.localScale.x * relativeXPos;

        // translate to Vector3
        Vector3 labelPos = new Vector3(quadXPos, quadYPos, -0.0001f);

        // set actual label
        setAxesLabel(i, yAxisLabelValues, labelPos);
      }

      // y axis grid (vertical)
      // get 1/nGridLines of lengthY, use modulo to make sure we don't get floats
      // this is not really necessary as the length should be already perfectly
      // divisble from our Start() function
      int yGridHeight = (lengthX - (lengthX % nGridLines)) / nGridLines;
      // draw grid lines
      for (int i = 0; i <= nGridLines; i++) {
        int xPos = border + i * yGridHeight;
        // length should be as long as y axis, make sure to increment by blockWidth
        // don't draw gridline for the first one
        if (i != 0) {
          for (int y = blockHeightLine; y <= lengthY; y += blockHeightGrid) {
            drawPixelBlock(xPos, border + y, blockWidthGrid, blockHeightGrid, colorsGrid);
          }
        }

        // set label
        // translate texture pixel to world space (Vector3)
        relativeXPos = (float)xPos / (float)texture.width;
        quadXPos = transform.localScale.x * relativeXPos;

        relativeYPos = (float)(border - 4 * blockHeightLine) / (float)texture.height;
        quadYPos = transform.localScale.y * relativeYPos;

        // translate to Vector3
        Vector3 labelPos = new Vector3(quadXPos, quadYPos, -0.0001f);

        // set actual label
        setAxesLabel(i , xAxisLabelValues, labelPos);
      }

      texture.Apply();
    }

    // creates a label for axes
    private void setAxesLabel(int number, float[] labelValues, Vector3 pos) {
      // instantiate label at the specific pos and parent
      GameObject newAxisLabel = (GameObject)Instantiate(AxisLabelPrefab, AxisLabelParentTransform);
      newAxisLabel.transform.localPosition = pos;

      // assign correct value to TextMeshPro
      TextMeshPro currentTMP = newAxisLabel.GetComponentInChildren<TextMeshPro>();
      // display decimal if necessary
      if (Mathf.Abs(labelValues[0]) < 1.0f && Mathf.Abs(labelValues[0]) > 0.0f) {
        currentTMP.SetText("{0:1}", labelValues[number]);
      } else {
        currentTMP.SetText("{0}", labelValues[number]);
      }
      
    }

    // creates arrays with axis label values based on max values as arguments
    private float[] createAxisLabelValues(float maxValue, float minValue) {
      // available grid steps:
      // 0.1, 0.2, 0.5,
      // 1, 2, 10, 20, 50,
      // 100, 200, 500,
      // 1000
      
      // divide max value by nGridLInes
      float gridDivider = maxValue / nGridLines;
      // always take the next biggest scale of maxValue / nGridLines

      float gridStep = predefinedAxisLabels.OrderBy(l => l).FirstOrDefault(l => l > gridDivider);

      // if no higher value found
      if (gridStep == 0f) {
        Debug.Log("GraphLine error: Max Value too high for predefinedAxisLabels");
        // default to 1000
        gridStep = 1000f;
      }

      return new float[] {0, gridStep, gridStep * 2, gridStep * 3, gridStep * 4, gridStep * 5};


    }

    // creates a line graph
    public void GraphLine(List<float> xValues, List<float> yValues, int _blockWidthLine, int _blockHeightLine, Color[] colors, string _xAxisTitle, string _yAxisTitle) {
      // check if lengths of x values and y values match, if not return
      if (xValues.Count != yValues.Count) {
        Debug.Log("GraphLine error: number of xValues and yValues need to be the same.");
        return;
      }

      blockWidthLine = _blockWidthLine;
      blockHeightLine = _blockHeightLine;
      xAxisTitle = _xAxisTitle;
      yAxisTitle = _yAxisTitle;

      // axis color (might take in as argument also)
      colorsAxis = Enumerable.Repeat<Color>(Color.gray, blockWidthLine * blockHeightLine).ToArray<Color>();

      // get maximum and minimum values of x and y axes (used to scale the graph correctly)
      float xMax = xValues.Max();
      float yMax = yValues.Max();
      float xMin = xValues.Min();
      float yMin = yValues.Min();

      // create arrays with axis label values
      xAxisLabelValues = createAxisLabelValues(xMax, xMin);
      yAxisLabelValues = createAxisLabelValues(yMax, yMin);

      float xLabelMax = xAxisLabelValues[nGridLines];
      float yLabelMax = yAxisLabelValues[nGridLines];

      // get ratio to length of axes
      float xFactor = (lengthX - blockWidthLine) / xLabelMax;
      float yFactor = (lengthY - blockHeightLine) / yLabelMax;

      // scale up / down values
      int[] scaledXValues = xValues.Select(x => (int)(x * xFactor)).ToArray();
      int[] scaledYValues = yValues.Select(x => (int)(x * yFactor)).ToArray();
      

      // draw x and y axes
      drawAxis("x", colorsAxis);
      drawAxis("y", colorsAxis);

      // set their description
      setAxesTitles();

      // draw grid
      drawGrid();

      // loop through xValues and call drawPixelBlock for each xValue-yValue pair
      // add border so that it's drawn within the axes
      for (int i = 0; i < scaledXValues.Length; i++) {
        drawPixelBlock(border + scaledXValues[i], border + scaledYValues[i], blockWidthLine, blockHeightLine, colors);
      }

      // upload to graphics card
      texture.Apply();
    }
  }
}
