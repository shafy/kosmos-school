using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kosmos
{
  // Creates a graph
  public class Grapher : MonoBehaviour {

    private int blockWidth;
    private int blockHeight;
    private int border;
    private int lengthX;
    private int lengthY;
    private Color[] colorsAxis;
    private Texture2D texture;
    private Renderer renderer;
    

    void Start() {
      renderer = GetComponent<Renderer>();

      // crate new texture
      texture = new Texture2D(256, 256);
      renderer.material.mainTexture = texture;

      // define colors array to be used with SetPixels block overload
      // the color is a flattened 2D array that has the length blockWidth * blockHeight
      blockWidth = 4;
      blockHeight = 4;
      colorsAxis = Enumerable.Repeat<Color>(Color.gray, blockWidth * blockHeight).ToArray<Color>();

      // define border size and lengths of axes
      border = 16;
      lengthX = texture.width - 2 * border;
      lengthY = texture.height - 2 * border;

      // draw x and y axes
      drawAxis("x",colorsAxis);
      drawAxis("y",colorsAxis);

      int[] exampleX = new int[5] {10, 15, 20, 25, 30};
      int[] exampleY = new int[5] {30, 30, 40, 60, 65};
      Color[] exampleColors = Enumerable.Repeat<Color>(Color.magenta, blockWidth * blockHeight).ToArray<Color>();
      GraphLine(exampleX, exampleY, exampleColors);

    }

    // draws a block of pixels (don't forget to call texture.Apply())
    private void drawPixelBlock(int x, int y, Color[] colors) {
      texture.SetPixels(x, y, blockWidth, blockHeight, colors, 0);
    }

    // draw x or y axis for graph
    private void drawAxis(string axis, Color[] colors) {
      if (axis == "x") {
        // draw line
        // draw pixel blocks, make sure to increase i by blockWidth
        for (int i = 0; i < lengthX; i += blockWidth) {
          drawPixelBlock(i + border, border, colors);
        }

        // draw arrow head
        // get last point using modus
        int lastPoint = border + lengthX - (lengthX % blockWidth);
        drawArrowHead("x", lastPoint, border);
      } else {
          // draw line
          // draw pixel blocks, make sure to increase i by blockHeight
          for (int i = 0; i < lengthY; i += blockHeight) {
            drawPixelBlock(border, i + border, colors);
          }

          // draw arrow head
          // get last point using modus
          int lastPoint = border + lengthY - (lengthY % blockHeight);
          drawArrowHead("y", border, lastPoint);
      }

      // upload texture to graphics card
      texture.Apply();
    }

    // draws arrow head for x or y axis
    private void drawArrowHead(string axis, int startingPointX, int startingPointY) {
      if (axis == "x") {
        // upper part
        drawPixelBlock(startingPointX - (2 * blockWidth), startingPointY + blockHeight, colorsAxis);
        drawPixelBlock(startingPointX - (3 * blockWidth), startingPointY + (2 * blockHeight), colorsAxis);

        // lower part
        drawPixelBlock(startingPointX - (2 * blockWidth), startingPointY - blockHeight, colorsAxis);
        drawPixelBlock(startingPointX - (3 * blockWidth), startingPointY - (2 * blockHeight), colorsAxis);
      } else {
        // right part
        drawPixelBlock(startingPointX + blockWidth, startingPointY - (2 * blockHeight), colorsAxis);
        drawPixelBlock(startingPointX + (2 * blockWidth), startingPointY - (3 * blockHeight), colorsAxis);

        // left part
        drawPixelBlock(startingPointX - blockWidth, startingPointY - (2 * blockHeight), colorsAxis);
        drawPixelBlock(startingPointX - (2 * blockWidth), startingPointY - (3 * blockHeight), colorsAxis);

        }
    }

    // creates a line graph
    public void GraphLine(int[] xValues, int[] yValues, Color[] colors) {
      // check if lengths of x values and y values match, if not return
      if (xValues.Length != yValues.Length) {
        Debug.Log("GraphLine error: number of xValues and yValues need to be the same.");
        return;
      }

      // loop through xValues and call drawPixelBlock for each xValue-yValue pair
      // add border so that it's drawn within the axes
      for (int i = 0; i < xValues.Length; i++) {
        drawPixelBlock(border + xValues[i], border + yValues[i], colors);
      }

      // upload to graphics card
      texture.Apply();
    }
  }
}
