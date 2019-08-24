using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using mixpanel;

namespace Kosmos {
  // controls for the welcome scene
  public class WelcomeGameControllers : MonoBehaviour {

    [SerializeField] private ControllerRayCaster controllerRayCaster;

    void Start() {
      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
       controllerRayCaster.RayCastEnabled = true;
       controllerRayCaster.EnableLineRenderer(true);
      }

      // once we have registered users we can use a better ID
      Mixpanel.Identify(getUniqueID());
      Mixpanel.People.Set("Headset", UnityEngine.XR.XRDevice.model);

      // set random name
      Mixpanel.People.SetOnce("$name", getRandomName());

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      Mixpanel.Track("Opened Scene", props);

      Mixpanel.StartTimedEvent("App Session");
    }

    // void OnApplicationQuit() {
    //   // track timed event end
    //   Mixpanel.Track("App Session");
    // }

    void Update() {
      // Button.Back is for Go, Button.Start for Quest
      if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
        if (OVRInput.GetDown(OVRInput.Button.Start)) {
          // minimize app
          OVRManager.PlatformUIConfirmQuit();
        }
      } else {
        if (OVRInput.GetDown(OVRInput.Button.Back)) {
          // minimize app
          OVRManager.PlatformUIConfirmQuit();
        }
      }
    }

    // generates or retrieves unique random ID used for Mixpanel People analytics
    private string getUniqueID() {
      // if there's already an ID, return it
      string existingID = PlayerPrefs.GetString("uniqueID", "");
      if (existingID != "") return existingID;

      // else, create new one, save it and return it
      System.Random random = new System.Random();
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      string uniqueID = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
      PlayerPrefs.SetString("uniqueID", uniqueID);

      return uniqueID;
    }

    // generates or retrieves random name for user
    private string getRandomName() {
      // if name exists, return it
      string existingName = PlayerPrefs.GetString("randomName", "");
      if (existingName != "") return existingName;

      // else, create a new one, save it and return it
      string[] randomNamesArrayFirst = new string[] {
        "Adorable",
        "Adventurous",
        "Annoyed",
        "Beautiful",
        "Blue",
        "Concerned",
        "Cute",
        "Dizzy",
        "Elegant",
        "Lively",
        "Stupid",
        "Talented",
        "Terrible",
        "Unusual",
        "Wicked"
      };
       string[] randomNamesArraySecond = new string[] {
        "Bird",
        "Dog",
        "Cat",
        "Horse",
        "Pig",
        "Raccoon",
        "Civet",
        "Ferret",
        "Aardvark",
        "Ant",
        "Baboon",
        "Camel",
        "Chicken",
        "Deer",
        "Dragonfly",
      };
      System.Random random = new System.Random();
      int randomNFirst = random.Next(0, randomNamesArrayFirst.Length);
      int randomNSecond = random.Next(0, randomNamesArraySecond.Length);

      string newName = $"{randomNamesArrayFirst[randomNFirst]} {randomNamesArraySecond[randomNSecond]}";
      PlayerPrefs.SetString("randomName", newName);
      return newName;
    }
  }
}
