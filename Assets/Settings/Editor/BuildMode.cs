using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Kosmos
{
  // Sets the Build Mode
  public class BuildMode {

    static string currentMode;

    [MenuItem("Kosmos/Dev Mode")]
    static void setDevMode() {
      setMode("dev");
    }

    [MenuItem("Kosmos/Prod Mode")]
    static void setProdMode() {
      setMode("prod");
    }

    static void setMode(string mode) {
      if (mode == "dev" && currentMode != "dev") {
        // set dev mode
        PlayerSettings.applicationIdentifier = "com.kosmosschool.KosmosSchoolcan";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        OVRManifestPreprocessor.RemoveAndroidManifest();
        setKeystore(false);
        setEntitlementCheck(false);
        currentMode = "dev";
        return;
      }

      if (mode == "prod" && currentMode != "prod") {
        // set prod mode
        PlayerSettings.applicationIdentifier = "com.kosmosschool.KosmosSchool";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.bundleVersion = (float.Parse(PlayerSettings.bundleVersion) + 0.01f).ToString("#.00");
        PlayerSettings.Android.bundleVersionCode = PlayerSettings.Android.bundleVersionCode + 1;
        OVRManifestPreprocessor.GenerateManifestForSubmission();
        setKeystore(true);
        setEntitlementCheck(true);
        currentMode = "prod";
        return;
      }
    }

    static void setKeystore(bool isEnabled) {
      if (isEnabled) {
        PlayerSettings.Android.keystoreName = EnvKeys.KEYSTORE_NAME;
        PlayerSettings.Android.keyaliasName = EnvKeys.KEYALIAS_NAME;
        PlayerSettings.Android.keyaliasPass = EnvKeys.KEYALIAS_PASS;
        PlayerSettings.Android.keystorePass = EnvKeys.KEYSTORE_PASS;
        return;
      }
      PlayerSettings.Android.keystoreName = "";
      PlayerSettings.Android.keyaliasName = "";
      PlayerSettings.Android.keyaliasPass = "";
      PlayerSettings.Android.keystorePass = "";
    }

    static void setEntitlementCheck(bool isEnabled) {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Shared/Scenes/WelcomeScreen.unity", OpenSceneMode.Additive);
        GameObject gameController = GameObject.Find("GameController");
        gameController.GetComponent<OculusPlatformSetup>().DoEntitlementCheck = isEnabled;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        EditorSceneManager.CloseScene(EditorSceneManager.GetActiveScene(), true);
    }
  }
}
