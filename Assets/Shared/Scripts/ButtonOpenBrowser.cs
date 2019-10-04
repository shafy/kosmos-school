using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kosmos;
using mixpanel;

namespace Kosmos.Shared {
  // when user clicks on this button it will open up the website in the browser
  public class ButtonOpenBrowser : MaterialButton {

    [SerializeField] private string url;
    [SerializeField] private string playerPrefsKey;
    
    public override void Press () {
      base.Press();

      var props = new Value();
      props["Scene Name"] = SceneManager.GetActiveScene().name;
      Mixpanel.Track("Clicked Open Browser Button", props);

      if (playerPrefsKey != "") PlayerPrefs.SetInt(playerPrefsKey, 1);
      
      Application.OpenURL(url);
    }
  }
}
