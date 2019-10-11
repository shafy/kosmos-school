using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.MagneticFields {
  // creates a cable based on a series of objects strung together by spring joints
  public class CableSpringJoint : MonoBehaviour {

    private float cableWidth;
    private float handleDistance;
    private float cableLength;
    private Grabbable currentGrabbable;
    private LineRenderer lineRenderer;
    private List<Transform> allSections;

    [SerializeField] private GameObject fixPoint;
    [SerializeField] private GameObject handle;

    void Start() {
      lineRenderer = GetComponent<LineRenderer>();

      cableWidth = 0.006f;
      allSections = new List<Transform>();

      // add all sections to allSections list
      Transform sections = transform.Find("Sections");

      foreach (Transform section in sections) {
        allSections.Add(section);
      }

      //currentGrabbable = handle.GetComponent<Grabbable>();

      cableLength = 2f;
    }

    void Update() {
      //positionHandle();
      displayCable();
    }

    // positions handle of cable (which the user grabs to move the end of the cable)
    // depending on if it's being grabbed and maximum length allowed 
    // private void positionHandle() {
    //   handleDistance = Vector3.Distance(fixPoint.transform.position, handle.transform.position);

    //   if (!currentGrabbable.IsPhantom && currentGrabbable.IsGrabbed && handleDistance >= cableLength) {
    //     currentGrabbable.IsPhantom = true;
    //   }

    //   if (currentGrabbable.IsPhantom) {
    //     Vector3 cableVectorNormal = (currentGrabbable.PhantomPosition - fixPoint.transform.position).normalized;
    //     Vector3 realPos = fixPoint.transform.position + cableVectorNormal * cableLength;
        
    //     float phantomDist = (currentGrabbable.PhantomPosition - fixPoint.transform.position).magnitude;

    //     if (phantomDist < cableLength) {
    //       currentGrabbable.IsPhantom = false;
    //     } else {
    //       handle.transform.position = fixPoint.transform.position + cableVectorNormal * cableLength;
    //     }
    //   }
    // }

    private void displayCable() {
      lineRenderer.startWidth = cableWidth;
      lineRenderer.endWidth = cableWidth;

      Vector3[] positions = new Vector3[allSections.Count];

      for (int i = 0; i < allSections.Count; i++) {
        positions[i] = allSections[i].position;
      }

      lineRenderer.positionCount = positions.Length;
      lineRenderer.SetPositions(positions);
    }
  }
}
