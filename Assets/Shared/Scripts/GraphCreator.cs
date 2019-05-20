using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kosmos
{ 
  // Creates Graphs based on argument data with CreateGraph
  public class GraphCreator : MonoBehaviour {

    [SerializeField] GameObject graphPrefab;
    [SerializeField] Transform graphsParentTransform;

    public void CreateGraph(List<GraphableData> graphableDataList, string xAxisTitle, string yAxisTitle) {

      // instantiate new graph object
      GameObject newGraph = (GameObject)Instantiate(graphPrefab, graphsParentTransform);

      Grapher currentGrapher = newGraph.GetComponentInChildren<Grapher>();
      // define params
      int blockWidthLine = 4;
      int blockHeightLine = 4;
      Color[] colorsMagenta = Enumerable.Repeat<Color>(Color.magenta, blockWidthLine * blockHeightLine).ToArray<Color>();

      currentGrapher.GraphLine(graphableDataList[0].XDataList, graphableDataList[0].YDataList, blockWidthLine, blockHeightLine, colorsMagenta, xAxisTitle, yAxisTitle);
    }
  }
}
