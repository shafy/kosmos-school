using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kosmos {
  // handles logic for the roller coaster builder
  public class RollerCoasterBuilderController : MonoBehaviour {

    private bool operationInProgress;
    private int currentItemIndex;
    private List<GameObject> previewItemsList;

    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro sizeText;

    public bool OperationInProgress {
      get { return operationInProgress; }
      set { operationInProgress = value; }
    }

    void Start() {
      currentItemIndex = 0;
      previewItemsList = new List<GameObject>();

      // get all preview items into list
      Transform previewParent = transform.Find("Previews");
      foreach (Transform child in previewParent) {
        previewItemsList.Add(child.gameObject);
      }

      // only display first item
      foreach (GameObject currentItem in previewItemsList) {
        currentItem.SetActive(false);
      }
      previewItemsList[0].SetActive(true);
      // set text name also
      RollerCoasterBuilderPreviewItem currentPreviewItem = previewItemsList[0].GetComponent<RollerCoasterBuilderPreviewItem>();
      SetItemNameTMP(currentPreviewItem.CurrentItemName);

      operationInProgress = false;
    }

    private void displayItem(int nextIndex, string direction) {
      // first, fade out current item
      fadeOutItem(currentItemIndex, direction);
      // second, fade in new item
      previewItemsList[nextIndex].SetActive(true);
      fadeInItem(nextIndex, direction);

      currentItemIndex = nextIndex;
    }

    private void fadeOutItem(int index, string direction) {
      GameObject item = previewItemsList[index];
      item.GetComponent<RollerCoasterBuilderPreviewItem>().FadeOut(direction);
    }

    private void fadeInItem(int index, string direction) {
      GameObject item = previewItemsList[index];
      RollerCoasterBuilderPreviewItem currentItem = item.GetComponent<RollerCoasterBuilderPreviewItem>();
      currentItem.FadeIn(direction);
      SetItemNameTMP(currentItem.CurrentItemName);
    }

    // switch to next or previous item
    public void SwitchItem(string direction) {
      // cancel if currently switching item
      if (operationInProgress) return;

      int nextIndex;
      if (direction == "next") {
        nextIndex = currentItemIndex + 1;
        // sanity check
        if (nextIndex == previewItemsList.Count) {
          nextIndex = 0;
        }
      } else {
        nextIndex = currentItemIndex - 1;
        // sanity check
        if (nextIndex < 0) {
          nextIndex = previewItemsList.Count - 1;
        }
      }

      // show next item
      displayItem(nextIndex, direction);
    }

    // changes the size of the item if available
    public void ChangeItemSize(string direction) {
      GameObject item = previewItemsList[currentItemIndex];
      RollerCoasterBuilderPreviewItem currentPreviewItem = item.GetComponent<RollerCoasterBuilderPreviewItem>();
      currentPreviewItem.ChangeSize(direction);
    }

    public void SetItemNameTMP(string text) {
      nameText.text = text;
    }

    public void SetItemSizeTMP(string text) {
      sizeText.text = text;
    }

    // places current item in scene
    public void PlaceCurrentItem() {
      RollerCoasterBuilderPreviewItem currentPreviewItem = previewItemsList[currentItemIndex].GetComponent<RollerCoasterBuilderPreviewItem>();
      currentPreviewItem.PlaceFullsizedItem();
    }
  }
}
