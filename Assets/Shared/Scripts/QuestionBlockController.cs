using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kosmos {
  // Main controls of Question Block logic
  public class QuestionBlockController : MonoBehaviour {

    private AudioSource audioSource;
    private bool isShowing;
    private bool firstClick;
    private GameObject mainCamera;
    private string name;
    private Vector3 initialPos;

    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;
    [SerializeField] private bool openAtStart = false;
    [SerializeField] private GameObject windowAndTexts;
    [SerializeField] private TextMeshPro textTitle;
    [SerializeField] private TextMeshPro textBody;
    [SerializeField] private GameObject clickedBlock;
    [SerializeField] private GameObject unclickedBlock;
    [SerializeField] private string titleText;
    [SerializeField, TextArea] private string bodyText;


    void Start() {
      mainCamera = GameObject.FindWithTag("MainCamera");

      initialPos = windowAndTexts.transform.position;

      isShowing = openAtStart;
      firstClick = !openAtStart;
      showWindow(openAtStart, openAtStart);
      unclickedBlock.SetActive(!openAtStart);
      clickedBlock.SetActive(openAtStart);

      textTitle.text = titleText;
      textBody.text = bodyText;

      audioSource = GetComponent<AudioSource>();

      name = gameObject.name;

      // has seen intro block before
      if (openAtStart && PlayerPrefs.GetInt($"{name}_hasSeenInstruction") == 1) {
        showWindow(false);
        unclickedBlock.active = false;
        clickedBlock.active = true;
        isShowing = false;
      }

      // hasn't seen before
      if (openAtStart && PlayerPrefs.GetInt($"{name}_hasSeenInstruction", 0) == 0) {
        showWindow(true, true);
        unclickedBlock.active = false;
        clickedBlock.active = true;

        PlayerPrefs.SetInt($"{name}_hasSeenInstruction", 1);
      }
    }

    private void showWindow(bool enabled, bool _openAtStart = false) {
      windowAndTexts.SetActive(enabled);

      if (!enabled || _openAtStart) {
        // reset position and rotation if disabling or if openAtStart
        windowAndTexts.transform.position = initialPos;
        windowAndTexts.transform.rotation = Quaternion.identity;
        return;
      }

      // rotate towards user on Y axis
      Vector3 lookPos = windowAndTexts.transform.position - mainCamera.transform.position;
      Quaternion tempRotation = Quaternion.LookRotation(lookPos);
      Quaternion newRotation = Quaternion.Euler(0, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z);
      windowAndTexts.transform.rotation = newRotation;

      // position 3 meters back, keep initial y pos
      Vector3 posVector = (clickedBlock.transform.position - mainCamera.transform.position).normalized;
      windowAndTexts.transform.position = mainCamera.transform.position + (posVector * 3f);
      windowAndTexts.transform.position = new Vector3(windowAndTexts.transform.position.x, initialPos.y, windowAndTexts.transform.position.z);
    }

    public void QuestionClick() {
      if (!isShowing) {
        showWindow(true);
        isShowing = true;
        if (firstClick) {
          unclickedBlock.active = false;
          clickedBlock.active = true;
          firstClick = false;
        }

        if (audioSource) {
          audioSource.clip = openClip;
          audioSource.Play();
        }
      } else {
        showWindow(false);
        isShowing = false;

        if (audioSource) {
          audioSource.clip = closeClip;
          audioSource.Play();
        }
      }
    }

  }
}
