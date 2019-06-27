using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // controls logic for preview items
  public class RollerCoasterBuilderPreviewItem : MonoBehaviour {

    private bool isFadingOut;
    private bool isFadingIn;
    private Color currentColor;
    private Dictionary<int, string> sizeToTextDict;
    private float startAlpha;
    private float endAlpha;
    private float t;
    private float fadeSpeed;
    private GameObject selectedSize;
    private int currentSizeIndex;
    private List<GameObject> sizesList;
    private string currentItemName;
    private Renderer selectedRenderer;
    private Vector3 initialPos;
    private Vector3 startPos;
    private Vector3 endPos;

    [SerializeField] private GameObject[] fullPrefabsArray;
    [SerializeField] private RollerCoasterBuilderController rollerCoasterBuilderController;
    [SerializeField] private string itemName;
    [SerializeField] private Transform nextInstantiationTransform;

    public string CurrentItemName {
      get { return currentItemName; }
      private set { currentItemName = value; }
    }

    void Awake() {
      startAlpha = 1.0f;
      endAlpha = 0.0f;
      isFadingOut = false;
      isFadingIn = false;
      t = 0.0f;
      fadeSpeed = 3.0f;

      initialPos = transform.position;

      // add all children
      sizesList = new List<GameObject>();
      foreach (Transform child in transform) {
        sizesList.Add(child.gameObject);
      }

      currentItemName = itemName;

      // crate sizeToTextDict
      sizeToTextDict = new Dictionary<int, string>();
      sizeToTextDict[0] = "Small";
      sizeToTextDict[1] = "Medium";
      sizeToTextDict[2] = "Large";

       // select the first one
      selectSize(0);
    }

    void Update() {
      if (isFadingOut) {
        t += Time.deltaTime * fadeSpeed;

        // change alpha
        currentColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
        selectedRenderer.material.color = currentColor;

        // move to the left
        //transform.Translate(Vector3.left * t);
        transform.position = Vector3.Lerp(initialPos, endPos, t);

        if (t > 1.0f) {
          // stop fading
          isFadingOut = false;
          gameObject.active = false;
          transform.position = initialPos;
          t = 0.0f;
          rollerCoasterBuilderController.OperationInProgress = false;
        }
      }

      if (isFadingIn) {
        t += Time.deltaTime * fadeSpeed;
        // change alpha
        currentColor.a = Mathf.Lerp(endAlpha, startAlpha, t);
        selectedRenderer.material.color = currentColor;

        // move to initial pos
        transform.position = Vector3.Lerp(startPos, initialPos, t);

        if (transform.position == initialPos) {
          isFadingIn = false;
          t = 0.0f;
          rollerCoasterBuilderController.OperationInProgress = false;
        }
      }

      rotateContinously();
    }

    // rotates object continously
    private void rotateContinously() {
      transform.Rotate(new Vector3(0, 1, 0), 20f * Time.deltaTime);
    }

    private void selectSize(int index) {
      selectedSize = sizesList[index];
      selectedRenderer = selectedSize.GetComponent<Renderer>();
      currentColor = selectedRenderer.material.color;
      currentColor.a = startAlpha;
      selectedRenderer.material.color = currentColor;
      
      // hide all others
      foreach (GameObject child in sizesList) {
        child.SetActive(false);
      }

      sizesList[index].SetActive(true);

      currentSizeIndex = index;

      // update text on display also
      rollerCoasterBuilderController.SetItemSizeTMP(sizeToTextDict[currentSizeIndex]);
    }

    // fades item out
    public void FadeOut(string direction) {
      // put startPos object x meters to the left or right
      // we take rollerCoasterBuilderController's right becaue this object is rotating
      if (direction == "next") {
        endPos = transform.position - rollerCoasterBuilderController.transform.right * 1f;
      } else {
        endPos = transform.position + rollerCoasterBuilderController.transform.right * 1f;
      }
     
      isFadingOut = true;
      rollerCoasterBuilderController.OperationInProgress = true;
    }

    public void FadeIn(string direction) {
      // put startPos object x meters to the right or left
      if (direction == "next") {
        startPos = transform.position + rollerCoasterBuilderController.transform.right * 1f;
      } else {
        startPos = transform.position - rollerCoasterBuilderController.transform.right * 1f;
      }

      isFadingIn = true;
      rollerCoasterBuilderController.OperationInProgress = true;

      // update text on display also
      rollerCoasterBuilderController.SetItemSizeTMP(sizeToTextDict[currentSizeIndex]);
    }

    public void ChangeSize(string direction) {
      int nextIndex;
      if (direction == "larger") {
        nextIndex = currentSizeIndex + 1;
        // sanity check
        if (nextIndex == sizesList.Count) {
          nextIndex = 0;
        }
      } else {
        nextIndex = currentSizeIndex - 1;
        // sanity check
        if (nextIndex < 0) {
          nextIndex = sizesList.Count - 1;
        }
      }

      selectSize(nextIndex);
    }

    // places full sized item in scene
    public void PlaceFullsizedItem() {
      //nextInstantiationTransform.position = new Vector3(0, 0, 0);
      GameObject fullItem = (GameObject)Instantiate(fullPrefabsArray[currentSizeIndex], nextInstantiationTransform, false);
    }
  }
}
