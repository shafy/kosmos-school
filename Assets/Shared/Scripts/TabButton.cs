using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // Activates connected Graph when pressed
  public class TabButton : Button {

    private bool isSelected;
    private GraphCreator graphCreator;
    private int graphIndex;
    private MeshRenderer meshRenderer;

    [SerializeField] Material selectedMaterial;
    [SerializeField] Material unselectedMaterial;

    void Awake() {
      isSelected = true;
      meshRenderer = GetComponent<MeshRenderer>();
    }

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

    public void SetSelected(bool selected) {
      if (selected == isSelected) return;

      if (selected) {
        meshRenderer.material = selectedMaterial;
      } else {
        meshRenderer.material = unselectedMaterial;
      }

      isSelected = selected;
    }
  }
}
