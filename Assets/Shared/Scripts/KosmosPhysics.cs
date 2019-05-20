using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Base class for Physics simulations
  public class KosmosPhysics : MonoBehaviour {

    private List<GraphableData> graphableDataList;

    [SerializeField] protected string xAxisTitle;
    [SerializeField] protected string yAxisTitle;

    public List<GraphableData> GraphableDataList {
      get { return graphableDataList; }
      protected set { graphableDataList = value; }
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
}
