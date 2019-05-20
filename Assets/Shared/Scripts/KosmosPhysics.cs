using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Base class for Physics simulations
  public class KosmosPhysics : MonoBehaviour {

    private List<GraphableData> graphableDataList;
    private List<GraphableDescription> graphableDescriptionList;

    public List<GraphableData> GraphableDataList {
      get { return graphableDataList; }
      protected set { graphableDataList = value; }
    }

    public List<GraphableDescription> GraphableDescriptionList {
      get { return graphableDescriptionList; }
      protected set { graphableDescriptionList = value; }
    }
  
  }

  // this class is used by the list of graphable data
  public class GraphableData {

    protected List<float> xDataList;
    protected List<float> yDataList;

    public List<float> XDataList {
      get { return xDataList; }
      private set { xDataList = value; }
    }

    public List<float> YDataList {
      get { return yDataList; }
      private set { yDataList = value; }
    }

    public GraphableData(List<float> _xDataList, List<float> _yDataList) {
      xDataList = _xDataList;
      yDataList = _yDataList;
    }
  }

  // this class is used by the list of graphable descriptions
  public class GraphableDescription {

    protected string graphTitle;
    protected string graphTabTitle;
    protected string xAxisTitle;
    protected string yAxisTitle;

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
