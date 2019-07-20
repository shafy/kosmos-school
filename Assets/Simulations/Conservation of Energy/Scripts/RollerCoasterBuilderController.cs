using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Kosmos {
  // handles logic for the roller coaster builder
  public class RollerCoasterBuilderController : MonoBehaviour {

    private bool operationInProgress;
    private bool cartHasBeenAdded;
    private int currentItemIndex;
    private List<GameObject> previewItemsList;
    private List <RollerCoasterItem.RCItemType> rcItemList;
    private List<RollerCoasterItem.RCItemType> addableList;
    private RollerCoasterItem.RCItemType itemTypeStartHill;
    private RollerCoasterItem.RCItemType itemTypeHill;
    private RollerCoasterItem.RCItemType itemTypeBankedCurve10;
    private RollerCoasterItem.RCItemType itemTypeBankedCurve15;
    private RollerCoasterItem.RCItemType itemTypeBankedCurve20;
    private RollerCoasterItem.RCItemType itemTypeLooping;
    private RollerCoasterItem.RCItemType itemTypeCart;
    private string startText;

    [SerializeField] private RCCartPortal rcCartPortal;
    [SerializeField] private RollerCoasterController rollerCoasterController;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro additionalText;
    [SerializeField] private TextMeshPro sizeText;

    public bool OperationInProgress {
      get { return operationInProgress; }
      set { operationInProgress = value; }
    }

    void Start() {
      startText = "Start by adding a Hill. Select a size with the plus and minus buttons. Press the round button to add it.";
      currentItemIndex = 0;
      previewItemsList = new List<GameObject>();
      cartHasBeenAdded = false;

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

      // assign the types to variables to keep them shorter
      itemTypeStartHill = RollerCoasterItem.RCItemType.StartHill;
      itemTypeHill = RollerCoasterItem.RCItemType.Hill;
      itemTypeBankedCurve10 = RollerCoasterItem.RCItemType.BankedCurve10;
      itemTypeBankedCurve15 = RollerCoasterItem.RCItemType.BankedCurve15;
      itemTypeBankedCurve20 = RollerCoasterItem.RCItemType.BankedCurve20;
      itemTypeLooping = RollerCoasterItem.RCItemType.Looping;
      itemTypeCart = RollerCoasterItem.RCItemType.Cart;

      rcItemList = new List<RollerCoasterItem.RCItemType>();
      addableList = new List<RollerCoasterItem.RCItemType>();
      updateAddableList();

      // set start instructions
      additionalText.text = startText;;
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
      RollerCoasterBuilderPreviewItem currentPreviewItem = item.GetComponent<RollerCoasterBuilderPreviewItem>();
      currentPreviewItem.FadeIn(direction);

      // check if current item is addable
      RollerCoasterItem currentFullItem = currentPreviewItem.GetCurrentFullSize().GetComponent<RollerCoasterItem>();
      updateItemAdditionalText(currentFullItem);

      SetItemNameTMP(currentPreviewItem.CurrentItemName);
    }

    private bool isAnyBankedCurve(RollerCoasterItem.RCItemType itemType) {
      if (itemType == itemTypeBankedCurve10 || itemType == itemTypeBankedCurve15 || itemType == itemTypeBankedCurve20) return true;
      return false;
    }

    private bool isHillOrLooping(RollerCoasterItem.RCItemType itemType) {
      if (itemType == itemTypeHill || itemType == itemTypeLooping) return true;
      return false;
    }

    // updates list of items that are currently addable
    private void updateAddableList() {

      // always clear list
      addableList = new List<RollerCoasterItem.RCItemType>();

      // if empty, only start hill an be added
      if (rcItemList.Count == 0) {
         addableList.Add(itemTypeHill);
         return;
      }

      // Special Case for cart:
      if (!cartHasBeenAdded) {
        addableList.Add(itemTypeCart);
      }
      
      // *** Main Case 1: Adding looping or hill after start hill ***

      // start hill -> next can be anything
      if (rcItemList.Count == 1) {
        addableList.Add(itemTypeHill);
        addableList.Add(itemTypeBankedCurve10);
        addableList.Add(itemTypeBankedCurve15);
        addableList.Add(itemTypeBankedCurve20);
        addableList.Add(itemTypeLooping);
        return;
      }

      // start hill + looping || hill -> next must be banked curve

      if (rcItemList.Count == 2 && isHillOrLooping(rcItemList[1])) {
        addableList.Add(itemTypeBankedCurve10);
        addableList.Add(itemTypeBankedCurve15);
        addableList.Add(itemTypeBankedCurve20);
        return;
      }

      // start hill + looping|| hill + banked curved -> next can't be banked curve
      if (rcItemList.Count == 3 && isHillOrLooping(rcItemList[1]) && isAnyBankedCurve(rcItemList[2])) {
        addableList.Add(itemTypeHill);
        addableList.Add(itemTypeLooping);
        return;
      }

      // start hill + looping|| hill + banked curved + looping || hill -> next can't be banked curve
      if (rcItemList.Count == 4 && isHillOrLooping(rcItemList[1]) && isAnyBankedCurve(rcItemList[2]) && isHillOrLooping(rcItemList[3])) {
        addableList.Add(itemTypeHill);
        addableList.Add(itemTypeLooping);
        return;
      }

      // start hill + looping|| hill + banked curved + looping || hill + looping || hill + looping
      // -> next must be banked curve of same radius as the first one
      if (rcItemList.Count == 5 && isHillOrLooping(rcItemList[1]) && isAnyBankedCurve(rcItemList[2]) && isHillOrLooping(rcItemList[3])) {
        addableList.Add(rcItemList[2]);
        return;
      }

      // if we have 6 items, no more items can be added
      if (rcItemList.Count == 6) return;

      // *** Main Case 2: Adding banked curve directly after start hill ***
      // start hill + banked curve -> next can't be banked curve
      if (rcItemList.Count == 2 && isAnyBankedCurve(rcItemList[1])) {
        addableList.Add(itemTypeHill);
        addableList.Add(itemTypeLooping);
        return;
      }

      // start hill + banked curve + looping || hill -> next must be banked curve of same radius as the first one
      if (rcItemList.Count == 3 && isAnyBankedCurve(rcItemList[1]) && isHillOrLooping(rcItemList[2])) {
        addableList.Add(rcItemList[1]);
        return;
      }

      // if we have 4 and already two banked curves, no more items can be added
      if (rcItemList.Count == 4 && isAnyBankedCurve(rcItemList[1]) && isHillOrLooping(rcItemList[2]) && isAnyBankedCurve(rcItemList[3])) {
        return;
      }
    }

    // checks if current item can be addeed to rollercoaster
    private bool isItemAddable(RollerCoasterItem.RCItemType itemType) {
      return addableList.Contains(itemType);
    }

    private void updateItemAdditionalText(RollerCoasterItem currentFullItem) {
      if (!isItemAddable(currentFullItem.ItemType)) {
        additionalText.text = "You can't add this part right now.";
      } else {
        if (rcItemList.Count == 0 && currentFullItem.ItemType == itemTypeHill) {
          additionalText.text = startText;;
        } else {
          additionalText.text = "";
        }
      }
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
      sizeText.text = "(" + text + ")";
    }

    // places current item in scene
    public void PlaceCurrentItem() {

      RollerCoasterBuilderPreviewItem currentPreviewItem = previewItemsList[currentItemIndex].GetComponent<RollerCoasterBuilderPreviewItem>();
      RollerCoasterItem currentFullItem = currentPreviewItem.GetCurrentFullSize().GetComponent<RollerCoasterItem>();

      if (!isItemAddable(currentFullItem.ItemType)) return;

      // special case: if it's the first item, make sure to add a start hill
      if (rcItemList.Count == 0) {
        currentPreviewItem.PlaceStartHill();
        rcItemList.Add(itemTypeStartHill);
        updateAddableList();
        // update text also
        additionalText.text = "Great! Now choose different parts with the left and right arrows.";
        return;
      }     

      Transform mostRecentElement = rollerCoasterController.ElementList.Last();
      currentPreviewItem.PlaceFullsizedItem(mostRecentElement);

      // don't update rcItemList for cart, instead update bool
      if (currentFullItem.ItemType == itemTypeCart) {
        cartHasBeenAdded = true;
      } else {
        rcItemList.Add(currentFullItem.ItemType);
      }

      updateItemAdditionalText(currentFullItem);
      
      updateAddableList();
    }

    public void AddElementToRC(Transform element, bool isStartHill = false) {
      rollerCoasterController.AddElement(element, isStartHill);
    }

    // removes most recent element
    public void RemoveLastItem() {
      if (rcItemList.Count == 0) {
        //additionalText.text = startText;
        return;
      }

      rollerCoasterController.RemoveElement();

      rcItemList.RemoveAt(rcItemList.Count - 1);

      if (rcItemList.Count == 0) {
        cartHasBeenAdded = false;
      }

      RollerCoasterBuilderPreviewItem currentPreviewItem = previewItemsList[currentItemIndex].GetComponent<RollerCoasterBuilderPreviewItem>();
      RollerCoasterItem currentFullItem = currentPreviewItem.GetCurrentFullSize().GetComponent<RollerCoasterItem>();

      updateItemAdditionalText(currentFullItem);
      
      updateAddableList();
    }

    // adds roller coaster cart reference to roller coaster controller and rc portal
    public void AddCartReference(RollerCoasterCart rollerCoasterCart) {
      rollerCoasterController.AddCartReference(rollerCoasterCart);
      rcCartPortal.AddCartReference(rollerCoasterCart);
    }
  }
}
