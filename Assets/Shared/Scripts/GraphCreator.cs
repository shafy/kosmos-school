using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Kosmos {

  // Handles Graph controls
  public class GraphCreator : MonoBehaviour {

    private List<GameObject> graphsList;
    private List<TabButton> graphTabButtonsList;
    private GraphableDataSet dataSet;

    [SerializeField] GameObject graphPrefab;
    [SerializeField] GameObject graphTabPrefab;
    [SerializeField] Transform graphsParentTransform;
    [SerializeField] Transform graphsTabParentTransform;


    void Awake() {
      graphsList = new List<GameObject>();
      graphTabButtonsList = new List<TabButton>();
      dataSet = new GraphableDataSet();
    }

    // creates empty graphs to be filled later
    public void CreateEmptyGraph(GraphableDescription graphableDescription) {
      dataSet.AddNewGraph(graphableDescription);
    }

    // adds data to dataset
    public void AddToDataSet(GraphableData newData, string graphTitle) {
      dataSet.AddData(newData, graphTitle);
    }

    // Creates Graphs and Tabs based on data in dataSet variable
    public void CreateGraph() {

      if (dataSet.GraphableDataListList.Count == 0) return;

      // define params
      int blockWidthLine = 4;
      int blockHeightLine = 4;
    
      // if graphableDataList and graphableDescription don't match, return
      if (dataSet.GraphableDataListList.Count != dataSet.GraphableDescriptionList.Count) {
        Debug.Log("CreateGraph Error: dataSet.GraphableDataListList and dataSet.graphableDescriptionList have different Counts");
        return;
      }

      for (int i = 0; i < dataSet.GraphableDataListList.Count; i++) {
        GameObject newGraph = (GameObject)Instantiate(graphPrefab, graphsParentTransform);
        Grapher currentGrapher = newGraph.GetComponentInChildren<Grapher>();

        // create new graph for each GraphableDataListList (x and y values list list)
        currentGrapher.GraphLines(dataSet.GraphableDataListList[i], dataSet.GraphableDescriptionList[i], blockWidthLine, blockHeightLine);

        // create tab also
        GameObject newTab = (GameObject)Instantiate(graphTabPrefab, graphsTabParentTransform);
        // move to right pos
        newTab.transform.localPosition = new Vector3(i * newTab.transform.localScale.x,  newTab.transform.localPosition.y, newTab.transform.localPosition.z);
        // update tab title
        TextMeshPro currentTMP = newTab.GetComponentInChildren<TextMeshPro>();
        currentTMP.text = dataSet.GraphableDescriptionList[i].GraphTabTitle;
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

    public void ClearGraphs() {
      // destroy graphs and tabs
      foreach (Transform child in graphsParentTransform) {
        Destroy(child.gameObject);
      }

      foreach (Transform child in graphsTabParentTransform) {
        Destroy(child.gameObject);
      }

      // clear dataset
      dataSet.Clear();
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
