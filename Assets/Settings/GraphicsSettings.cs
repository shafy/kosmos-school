using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
    public class GraphicsSettings : MonoBehaviour {

      void Start() {
        // set FFR (fixed foveated rendering) to high
        OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSHigh;

        // set chromatic de-aberration to true
        OVRPlugin.chromatic = true;

        // set refresh reate
        //OVRManager.display.displayFrequency = 72.0f;

        // gpu and cpu clock throttling
        OVRPlugin.cpuLevel = 2;
        OVRPlugin.gpuLevel = 2;
      }      
    }

}