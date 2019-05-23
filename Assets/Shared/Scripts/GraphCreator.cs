using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Kosmos
{ 
  // Handles Graph controls
  public class GraphCreator : MonoBehaviour {

    private List<GameObject> graphsList;
    private List<TabButton> graphTabButtonsList;

    [SerializeField] GameObject graphPrefab;
    [SerializeField] GameObject graphTabPrefab;
    [SerializeField] Transform graphsParentTransform;
    [SerializeField] Transform graphsTabParentTransform;


    void Awake() {
      graphsList = new List<GameObject>();
      graphTabButtonsList = new List<TabButton>();
    }

    // Creates Graphs and Tabs
    public void CreateGraph(List<GraphableData> graphableDataList, List<GraphableDescription> graphableDescription) {

      // define params
      int blockWidthLine = 4;
      int blockHeightLine = 4;
      Color[] colorsMagenta = Enumerable.Repeat<Color>(Color.magenta, blockWidthLine * blockHeightLine).ToArray<Color>();

      // if graphableDataList and graphableDescription don't match, return
      if (graphableDataList.Count != graphableDescription.Count) {
        Debug.Log("CreateGraph Error: graphableDataList and graphableDescription have different Counts");
        return;
      }

      // for each y and x data pair in graphableDataList, create new graph and graph line
      for (int i = 0; i < graphableDataList.Count; i++) {
        GameObject newGraph = (GameObject)Instantiate(graphPrefab, graphsParentTransform);
        Grapher currentGrapher = newGraph.GetComponentInChildren<Grapher>();

        currentGrapher.GraphLine(graphableDataList[i].XDataList, graphableDataList[i].YDataList, blockWidthLine, blockHeightLine, colorsMagenta, graphableDescription[i].XAxisTitle, graphableDescription[i].YAxisTitle);

        // create tab also
        GameObject newTab = (GameObject)Instantiate(graphTabPrefab, graphsTabParentTransform);
        // move to right pos
        newTab.transform.localPosition = new Vector3(i * newTab.transform.localScale.x,  newTab.transform.localPosition.y, newTab.transform.localPosition.z);
        // update tab title
        TextMeshPro currentTMP = newTab.GetComponentInChildren<TextMeshPro>();
        currentTMP.text = graphableDescription[i].GraphTabTitle;
        // assign index and this instance of the GraphCreator
        TabButton currentTabButton = newTab.GetComponent<TabButton>();
        currentTabButton.GraphIndex = i;
        currentTabButton.GraphCreator = this;

        // add to list
        graphsList.Add(newGraph);
        graphTabButtonsList.Add(currentTabButton);

        // disable if not first
        if (i > 0) {
          newGraph.active = false;
          currentTabButton.SetSelected(false);
        }

      }
    }

    // switches to new graph based on tab button press
    public void SwitchGraphs(int newGraphIndex) {
      // loop through all objects in the list and deactivate
      for (int i = 0; i < graphsList.Count; i++) {
        graphsList[i].active = false;
        graphTabButtonsList[i].SetSelected(false);
      }
      // activate the new one
      graphsList[newGraphIndex].active = true;
      graphTabButtonsList[newGraphIndex].SetSelected(true);
    }
  }
}
