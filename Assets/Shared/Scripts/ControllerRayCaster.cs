/************************************************************************************
This script is based on and is an adaptation of VREyeRayCaster.cs distributed with the
Unity Oculus Integration.
************************************************************************************/

using System;
using UnityEngine;

namespace Kosmos
{
    // In order to interact with objects in the scene
    // this class casts a ray into the scene and if it finds
    // a InteractiveItem it exposes it for other classes to use.
    // This script should be generally be placed on the camera.
    public class ControllerRayCaster : MonoBehaviour
    {
        private InteractiveItem m_CurrentInteractible;                //The current interactive item
        private InteractiveItem m_LastInteractible;                   //The last interactive item
        private RaycastHit currentHit;
        private QueryTriggerInteraction currentQuerryTriggerInteraction;

        [SerializeField] private Transform m_Camera;
        [SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
        //[SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
        [SerializeField] private GazePointer m_Reticle;
        [SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
        [SerializeField] private float m_DebugRayLength = 5f;           // Debug ray length.
        [SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
        [SerializeField] private float m_RayLength = 5f;              // How far into the scene the ray is cast.
         // Tracking space (for line renderer)
        [SerializeField] private Transform m_TrackingSpace = null;

        // new custom fields to have laser shoot out from controller
        // For supporting Laser Pointer
        [SerializeField] private LineRenderer m_LineRenderer = null;
        // Laser pointer visibility
        public bool ShowLineRenderer = false;
        public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.

        // we make this public to access it from the HandleLaserInteraction script of the interactible object
        public Vector3 worldStartPoint;
        public Vector3 worldEndPoint;
        private Vector3 worldEndPontLineRenderer;
        private Transform leftHand;
        public bool RayCastEnabled = false;

        [SerializeField] private Transform leftHandIndex;

        // we added the following two methods to get the currently connected controller
        public bool ControllerIsConnected {
            get {
                if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
                    OVRInput.Controller controller = OVRInput.GetConnectedControllers();
                    // return true if LTouch is connected (don't check for RTouch for now)
                    if ((controller & OVRInput.Controller.LTouch) == OVRInput.Controller.LTouch) {
                        return true;
                    }
                    return false;
                } else {
                    // return true if either RTrackedRemote or LTrackedRemote is connected
                    OVRInput.Controller controller = OVRInput.GetConnectedControllers() & (OVRInput.Controller.LTrackedRemote | OVRInput.Controller.RTrackedRemote);
                    return controller == OVRInput.Controller.LTrackedRemote || controller == OVRInput.Controller.RTrackedRemote;
                }
                
            }
        }

        // Utility for other classes to get the current interactive item
        public InteractiveItem CurrentInteractible
        {
            get { return m_CurrentInteractible; }
        }

        public RaycastHit CurrentHit {
            get { return currentHit; }
            private set { currentHit = value;}
        }

        public QueryTriggerInteraction CurrentQuerryTriggerInteraction {
            get { return currentQuerryTriggerInteraction; }
            set { currentQuerryTriggerInteraction = value;}
        }

        private void Start() {
            // ignore trigger colliders per default
            currentQuerryTriggerInteraction = QueryTriggerInteraction.Ignore;
            leftHand = GameObject.FindWithTag("LeftHandAnchor").transform.Find("HandLeft");
        }

        private void Update()
        {  
            if (RayCastEnabled) EyeRaycast();
        }

      
        private void EyeRaycast()
        {
            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);
            RaycastHit hit;

            // custom laser controller code
            worldStartPoint = Vector3.zero;
            worldEndPoint = Vector3.zero;

            // enable line renderer component if controller is connected
            if (m_LineRenderer != null) {
                m_LineRenderer.enabled = ControllerIsConnected && ShowLineRenderer;
            }

            // if controller connected, create a laser pointer
            if (ControllerIsConnected && m_TrackingSpace != null) {
                Matrix4x4 localToWorld = m_TrackingSpace.localToWorldMatrix;
               

                OVRInput.Controller currentController;
                
                if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
                    currentController = OVRInput.Controller.LTouch;
                } else {
                    currentController = KosmosStatics.Controller;
                }

                Quaternion orientation = OVRInput.GetLocalControllerRotation(currentController);

                if (UnityEngine.XR.XRDevice.model == "Oculus Quest") {
                    worldStartPoint = leftHandIndex.position;
                    worldEndPoint = worldStartPoint + ((orientation * Vector3.forward) * m_RayLength);
                    worldEndPontLineRenderer = worldStartPoint + ((orientation * Vector3.forward) * 2.0f);
                } else {
                    Vector3 localStartPoint = OVRInput.GetLocalControllerPosition(currentController);
                    Vector3 localEndPoint = localStartPoint + ((orientation * Vector3.forward) * m_RayLength);
                    Vector3 localEndPointLineRenderer = localStartPoint + ((orientation * Vector3.forward) * 2.0f);

                    worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);
                    worldEndPoint = localToWorld.MultiplyPoint(localEndPoint);
                    worldEndPontLineRenderer = localToWorld.MultiplyPoint(localEndPointLineRenderer);
                }

                // create new ray
                ray = new Ray(worldStartPoint, worldEndPoint - worldStartPoint);
            }
            
            // Do the raycast forweards to see if we hit an interactive item
            // ignore colliders that have isTrigger = true
            if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers, currentQuerryTriggerInteraction))
            {
                InteractiveItem interactible = hit.collider.GetComponent<InteractiveItem>(); //attempt to get the InteractiveItem on the hit object

                currentHit = hit;
                // if laser hits an object, make sure that the endpoint is where it hits it
                if (interactible) {
                    worldEndPoint = hit.point;
                }
                    //DisplayReticle(hit);
                // } else {
                //     if (m_Reticle) {
                //         m_Reticle.Hide();
                //     }
                // }

                m_CurrentInteractible = interactible;

                // If we hit an interactive item and it's not the same as the last interactive item, then call Over
                if (interactible && interactible != m_LastInteractible) {
                    interactible.Over(); 
                }

                // Deactive the last interactive item 
                if (interactible != m_LastInteractible)
                    DeactiveLastInteractible();

                m_LastInteractible = interactible;

                if (OnRaycasthit != null)
                    OnRaycasthit(hit);

                // show reticle if hit UI layer
                if (hit.transform.gameObject.layer == 5){
                    //DisplayReticle(hit);
                } else {
                    if (m_Reticle) {
                        m_Reticle.Hide();
                        //m_Reticle.SetPosition(ray.origin, ray.direction);
                    }
                }
            }
            else
            {
                // Nothing was hit, deactive the last interactive item.
                DeactiveLastInteractible();
                m_CurrentInteractible = null;

                currentHit = new RaycastHit();

                // hide the reticle
                if (m_Reticle) {
                    m_Reticle.Hide();
                    //m_Reticle.SetPosition(ray.origin, ray.direction);
                }
                     
            }

            // set the linerenderer's points to the same ones as the ray
            if (ControllerIsConnected && m_LineRenderer != null && m_LineRenderer.enabled) {
                m_LineRenderer.SetPosition(0, worldStartPoint);
                m_LineRenderer.SetPosition(1, worldEndPontLineRenderer);
            }
        }

        private void DisplayReticle(RaycastHit hit) {
            if (m_Reticle) {
                m_Reticle.Show();
                m_Reticle.SetPosition(hit.point);
            }
        }

        public void EnableLineRenderer(bool isEnabled) {
            ShowLineRenderer = isEnabled;
            m_LineRenderer.enabled = isEnabled;
        }


        private void DeactiveLastInteractible()
        {
            if (m_LastInteractible == null)
                return;

            m_LastInteractible.Out();
            m_LastInteractible = null;
        }
    }
}
