using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kosmos.Shared;

namespace Kosmos.MagneticFields {
  // takes care of magnetic fields controls
  public class MFController : MonoBehaviour {

    private float prevSliderValue;
    [SerializeField] private SliderControl sliderControl;
    [SerializeField] private ConductorMF conductorMF;

    void Start() {
      prevSliderValue = sliderControl.SliderValue;
    }

    void Update() {
      if (prevSliderValue == sliderControl.SliderValue) return;

      conductorMF.SetCurrent(sliderControl.SliderValue);
      prevSliderValue = sliderControl.SliderValue;
    } 
  
  }
}
