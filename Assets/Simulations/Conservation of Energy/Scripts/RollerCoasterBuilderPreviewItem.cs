using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // controls logic for preview items
  public class RollerCoasterBuilderPreviewItem : MonoBehaviour {

    private AudioSource audioSource;
    private bool isFadingOut;
    private bool isFadingIn;
    private Color currentColor;
    private Dictionary<int, string> sizeToTextDict;
    private float startAlpha;
    private float endAlpha;
    private float t;
    private float fadeSpeed;
    private float switchDistance;
    private GameObject selectedSize;
    private int currentSizeIndex;
    private List<GameObject> sizesList;
    private string currentItemName;
    private Renderer selectedRenderer;
    private Vector3 initialPos;
    private Vector3 startPos;
    private Vector3 endPos;

    [SerializeField] private GameObject[] fullPrefabsArray;
    // starthill prefabs are only needed by the hill previewitem
    [SerializeField] private GameObject[] startHillPrefabs;
    [SerializeField] private GameObject particleSystemPrefab;
    [SerializeField] private RollerCoasterBuilderController rollerCoasterBuilderController;
    [SerializeField] private string itemName;

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
      switchDistance = 2.0f;

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

      audioSource = GetComponent<AudioSource>();
    }

    void Update() {
      if (isFadingOut) {
        t += Time.deltaTime * fadeSpeed;

        // change alpha
        currentColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
        selectedRenderer.material.color = currentColor;

        // move to the left
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

    private IEnumerator PlaceItemCoroutine(GameObject currentGO) {
      rollerCoasterBuilderController.OperationInProgress = true;
      yield return new WaitForSeconds(2.0f);
      currentGO.SetActive(true);
      rollerCoasterBuilderController.OperationInProgress = false;
    }

    // fades item out
    public void FadeOut(string direction) {
      // put startPos object x meters to the left or right
      // we take rollerCoasterBuilderController's right becaue this object is rotating
      if (direction == "next") {
        endPos = transform.position - rollerCoasterBuilderController.transform.right * switchDistance;
      } else {
        endPos = transform.position + rollerCoasterBuilderController.transform.right * switchDistance;
      }
     
      isFadingOut = true;
      rollerCoasterBuilderController.OperationInProgress = true;
    }

    public void FadeIn(string direction) {
      // put startPos object x meters to the right or left
      if (direction == "next") {
        startPos = transform.position + rollerCoasterBuilderController.transform.right * switchDistance;
      } else {
        startPos = transform.position - rollerCoasterBuilderController.transform.right * switchDistance;
      }

      isFadingIn = true;
      rollerCoasterBuilderController.OperationInProgress = true;

      // update text on display also
      rollerCoasterBuilderController.SetItemSizeTMP(sizeToTextDict[currentSizeIndex]);
    }

    public int ChangeSize(string direction) {
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
      return nextIndex;
    }

    public GameObject GetCurrentFullSize() {
      return fullPrefabsArray[currentSizeIndex]; 
    }

    // places full sized item in scene
    public void PlaceFullsizedItem(Transform mostRecentElement) {
      GameObject fullItem = (GameObject)Instantiate(GetCurrentFullSize(), new Vector3(0, 0, 0), Quaternion.identity);

      if (currentItemName != "Cart") {
        // get connectors
        Transform connectorBackwardNew = fullItem.transform.Find("Connector Backward");
        Transform connectorForwardAOld = mostRecentElement.Find("Connector Forward A");
        Transform connectorForwardBOld = mostRecentElement.Find("Connector Forward B");

        // find out direction vector and apply rotation
        Vector3 directionVec = (connectorForwardBOld.position - connectorForwardAOld.position).normalized;

        Quaternion newRotation = Quaternion.FromToRotation(Vector3.forward, directionVec);
        fullItem.transform.rotation = Quaternion.Euler(fullItem.transform.rotation.eulerAngles.x, newRotation.eulerAngles.y, fullItem.transform.rotation.eulerAngles.z);

        // based on the most recently placed element (before this one), find out where to place the current one
        // apply same rotation to new item
        Vector3 vectorConnectorBackwardToCenter = fullItem.transform.position - connectorBackwardNew.position;

        // apply this vector to connectorForwardOld
        Vector3 newPos = connectorForwardAOld.position + vectorConnectorBackwardToCenter;

        fullItem.transform.position = newPos;

        // add item to list for roller coaster
        rollerCoasterBuilderController.AddElementToRC(fullItem.transform);

        // also instantiate particle system
        GameObject particleSystem = (GameObject)Instantiate(particleSystemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        particleSystem.transform.position = newPos;
        // play building audio
        audioSource.Play();

        // deactivate item and start coroutine to re-activate it
        fullItem.SetActive(false);
        StartCoroutine(PlaceItemCoroutine(fullItem));

      } else {
        // in this case it's a cart
        RollerCoasterCart rollerCoasterCart = fullItem.GetComponent<RollerCoasterCart>();
        rollerCoasterBuilderController.AddCartReference(rollerCoasterCart);
      }
    }

    // places full sized Start Hill (this is the first item an a special case of PlaceFullsizedItem())
    public void PlaceStartHill() {
      GameObject fullItem = (GameObject)Instantiate(startHillPrefabs[currentSizeIndex], new Vector3(0, 0, 0), Quaternion.identity);
      rollerCoasterBuilderController.AddElementToRC(fullItem.transform, true);

      // also instantiate particle system
      GameObject particleSystem = (GameObject)Instantiate(particleSystemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
      particleSystem.transform.position = fullItem.transform.position;
      // play building audio
      audioSource.Play();

      // deactivate item and start coroutine to re-activate it
      fullItem.SetActive(false);
      StartCoroutine(PlaceItemCoroutine(fullItem));
    }
  }
}
