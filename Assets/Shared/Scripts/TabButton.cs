using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Activates connected Graph when pressed
  public class TabButton : Button {

    private GraphCreator graphCreator;
    private int graphIndex;

    public GraphCreator GraphCreator {
      get { return graphCreator; }
      set { graphCreator = value; }
    }

    public int GraphIndex {
      get { return graphIndex; }
      set { graphIndex = value; }
    }

    public override void Press () {
      graphCreator.SwitchGraphs(graphIndex);
    }
  }
}
