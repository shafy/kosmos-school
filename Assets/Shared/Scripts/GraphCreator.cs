using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kosmos
{ 
  // Creates Graphs based on argument data with CreateGraph
  public class GraphCreator : MonoBehaviour {

    [SerializeField] GameObject graphPrefab;

    public void CreateGraph(List<GraphableData> graphableDataList) {

      // instantiate new graph object
      GameObject newGraph = graphPrefab;

      Grapher currentGrapher = newGraph.GetComponentInChildren<Grapher>();
      int blockWidthLine = 4;
      int blockHeightLine = 4;
      Color[] colorsMagenta = Enumerable.Repeat<Color>(Color.magenta, blockWidthLine * blockHeightLine).ToArray<Color>();

      currentGrapher.GraphLine(graphableDataList[0].XDataList, graphableDataList[0].YDataList, blockWidthLine, blockHeightLine, colorsMagenta);
    }
  }
}
