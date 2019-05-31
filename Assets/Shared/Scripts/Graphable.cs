using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kosmos {
  // these classes define data types and methods used to graph data

  public class GraphableDataSet {
    private List<List<GraphableData>> graphableDataListList;
    private List<GraphableDescription> graphableDescriptionList;

    public List<List<GraphableData>> GraphableDataListList {
      get { return graphableDataListList; }
      private set { graphableDataListList = value; }
    }

    public List<GraphableDescription> GraphableDescriptionList {
      get { return graphableDescriptionList; }
      private set { graphableDescriptionList = value; }
    }

    public GraphableDataSet() {
      // initialize lists
      graphableDataListList = new List<List<GraphableData>>();
      graphableDescriptionList = new List<GraphableDescription>();
    }

    // adds new GraphableData to DataSet matching by GraphableDescription.GraphTitle
    public void AddData(GraphableData newData, string graphTitle) {
      // search through GraphableDescriptions
      // this can be made more efficient in the future
      for (int i = 0; i < graphableDescriptionList.Count; i++) {
        if (graphableDescriptionList[i].GraphTitle == graphTitle) {
          // add data to graphableDataListList and stop for loop
          graphableDataListList[i].Add(newData);
          return;
        }
      }

      Debug.Log("AddData: No Graph found with GrahTitle: " + graphTitle);
    }
    // adds a new graph
    public void AddNewGraph(GraphableDescription _graphableDescription) {
      // future: add some validation
      graphableDescriptionList.Add(_graphableDescription);
      // also add a new empty List to ListList that will later be filled by AddData
      graphableDataListList.Add(new List<GraphableData>());
    }

    public void Clear() {
      graphableDataListList = new List<List<GraphableData>>();
      graphableDescriptionList = new List<GraphableDescription>();
    }
  }

  // this class is used by the list of graphable data
  public class GraphableData {

    private Color color;
    private List<float> xDataList;
    private List<float> yDataList;
    private string name;

    public Color Color {
      get { return color; }
      private set { color = value; }
    }

    public List<float> XDataList {
      get { return xDataList; }
      private set { xDataList = value; }
    }

    public List<float> YDataList {
      get { return yDataList; }
      private set { yDataList = value; }
    }

    public string Name {
      get { return name; }
      private set { name = value; }
    }

    public GraphableData(List<float> _xDataList, List<float> _yDataList, string _name, Color _color) {
      xDataList = _xDataList;
      yDataList = _yDataList;
      name = _name;
      color = _color;
    }
  }

  // this class is used by the list of graphable descriptions
  public class GraphableDescription {

    private string graphTitle;
    private string graphTabTitle;
    private string xAxisTitle;
    private string yAxisTitle;

    public string GraphTitle {
      get { return graphTitle; }
      private set { graphTitle = value; }
    }

    public string GraphTabTitle {
      get { return graphTabTitle; }
      private set { graphTabTitle = value; }
    }

    public string XAxisTitle {
      get { return xAxisTitle; }
      private set { xAxisTitle = value; }
    }

    public string YAxisTitle {
      get { return yAxisTitle; }
      private set { yAxisTitle = value; }
    }

    public GraphableDescription(string _graphTitle, string _graphTabTitle, string _xAxisTitle, string _yAxisTitle) {
      graphTitle = _graphTitle;
      graphTabTitle = _graphTabTitle;
      xAxisTitle = _xAxisTitle;
      yAxisTitle = _yAxisTitle;
    }
  }
}