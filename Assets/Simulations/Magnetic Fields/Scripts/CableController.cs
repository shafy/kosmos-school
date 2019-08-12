using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos {
  // cable based on rope from https://www.habrador.com/tutorials/rope/2-simplified-rope/
  public class CableController : MonoBehaviour {
    //Objects that will interact with the rope
    public Transform whatTheRopeIsConnectedTo;
    public Transform whatIsHangingFromTheRope;

    //Line renderer used to display the rope
    private LineRenderer lineRenderer;

    //A list with all rope section
    private List<RopeSection> allRopeSections = new List<RopeSection>();

    //Rope data
    private float sectionLength = 0.2f;
    private float maxLength;
    private Grabbable currentGrabbable;
    private int nSections;

    private void Start() 
    {
        //Init the line renderer we use to display the rope
        lineRenderer = GetComponent<LineRenderer>();

        sectionLength = 0.2f;
        nSections = 10;
        maxLength = sectionLength * nSections;


        //Create the rope
        Vector3 ropeSectionPos = whatTheRopeIsConnectedTo.position;

        for (int i = 0; i < nSections; i++)
        {
            allRopeSections.Add(new RopeSection(ropeSectionPos));

            ropeSectionPos.y -= sectionLength;
        }

        whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;

        currentGrabbable = whatIsHangingFromTheRope.GetComponent<Grabbable>();
    }
    
    private void Update() 
    {
        //Display the rope with the line renderer
        DisplayRope();

        // if (currentGrabbable.IsGrabbed) {
        //     RopeSection lastRopeSection = allRopeSections[allRopeSections.Count - 1];
        //     lastRopeSection.pos = whatIsHangingFromTheRope.position;
        //     allRopeSections[allRopeSections.Count - 1] = lastRopeSection;
        // } else {
        //     whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;
        //     whatIsHangingFromTheRope.LookAt(allRopeSections[allRopeSections.Count - 2].pos);
        // }

        //Move what is hanging from the rope to the end of the rope
        //whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;

        //Make what's hanging from the rope look at the next to last rope position to make it rotate with the rope
        //whatIsHangingFromTheRope.LookAt(allRopeSections[allRopeSections.Count - 2].pos);

        // get distance of last section and whatIsHangingFromTheRope
        // float currentDistance = Vector3.Distance(allRopeSections[allRopeSections.Count - 1].pos, whatIsHangingFromTheRope.position);

        // if (currentDistance > 0.001f) {
        //     whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;
        // }
        

        // float currentDistance = Vector3.Distance(whatTheRopeIsConnectedTo.position, whatIsHangingFromTheRope.position);

        // if (currentDistance > 3f && currentGrabbable.IsGrabbed) {
        //     // we take the phantom pos because we want to make sure that the end of the cable moves in that direction
        //     currentGrabbable.IsPhantom = true;
        //     Vector3 cableVectorNormal = (currentGrabbable.PhantomPosition - whatTheRopeIsConnectedTo.position).normalized;

        //     Vector3 newPos = whatTheRopeIsConnectedTo.position + cableVectorNormal * 3.1f;
        //     whatIsHangingFromTheRope.position = newPos;

        //     //currentGrabbable.IsGrabbable = false;

        // } else if (currentDistance > 3f && !currentGrabbable.IsGrabbed) {
        //     whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;

        // } else {
        //     currentGrabbable.IsPhantom = false;

        // }

        UpdateRopeSimulation();

    }

    private void FixedUpdate()
    {
        //UpdateRopeSimulation();
    }

    private void UpdateRopeSimulation()
    {
        Vector3 gravityVec = new Vector3(0f, -9.81f, 0f);

        float t = Time.fixedDeltaTime;


        //Move the first section to what the rope is hanging from
        RopeSection firstRopeSection = allRopeSections[0];

        firstRopeSection.pos = whatTheRopeIsConnectedTo.position;

        allRopeSections[0] = firstRopeSection;


        // Move the other rope sections with Verlet integration
        // all except last one
        // int count = allRopeSections.Count;
        // if (currentGrabbable.IsGrabbed) {
        //     count =- 1;
        // }
        for (int i = 1; i < allRopeSections.Count - 1; i++)
        {
            RopeSection currentRopeSection = allRopeSections[i];

            //Calculate velocity this update
            Vector3 vel = currentRopeSection.pos - currentRopeSection.oldPos;

            //Update the old position with the current position
            currentRopeSection.oldPos = currentRopeSection.pos;

            //Find the new position
            currentRopeSection.pos += vel;

            //Add gravity
            currentRopeSection.pos += gravityVec * t;

            //Add it back to the array
            allRopeSections[i] = currentRopeSection;
        }

        // move the last one to whatIsHangingFromTheRope's pos

        // RopeSection lastRopeSection = allRopeSections[allRopeSections.Count - 1];
        // lastRopeSection.pos = whatIsHangingFromTheRope.position;
        // allRopeSections[allRopeSections.Count - 1] = lastRopeSection;

        RopeSection lastRopeSection = allRopeSections[allRopeSections.Count - 1];
        lastRopeSection.pos = whatIsHangingFromTheRope.position;
        allRopeSections[allRopeSections.Count - 1] = lastRopeSection;



        //Make sure the rope sections have the correct lengths
        for (int i = 0; i < 20; i++)
        {
            ImplementMaximumStretch();
        }

        float currentDistance = Vector3.Distance(whatTheRopeIsConnectedTo.position, whatIsHangingFromTheRope.position);

        // if (currentGrabbable.IsGrabbed) {
        //     RopeSection lastRopeSection = allRopeSections[allRopeSections.Count - 1];
        //     lastRopeSection.pos = whatIsHangingFromTheRope.position;
        //     allRopeSections[allRopeSections.Count - 1] = lastRopeSection;
        // } else {
        //     whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;
        // }

        // if (currentDistance > maxLength) {
        //     Vector3 cableVectorNormal = (currentGrabbable.transform.position - whatTheRopeIsConnectedTo.position).normalized;
        //     Vector3 newPos = whatTheRopeIsConnectedTo.position + cableVectorNormal * maxLength;
        //     whatIsHangingFromTheRope.position = newPos;
        // }


        // if (currentDistance > 3f && currentGrabbable.IsGrabbed) {
        //     // we take the phantom pos because we want to make sure that the end of the cable moves in that direction
        //     currentGrabbable.IsPhantom = true;
        //     Vector3 cableVectorNormal = (currentGrabbable.PhantomPosition - whatTheRopeIsConnectedTo.position).normalized;

        //     Vector3 newPos = whatTheRopeIsConnectedTo.position + cableVectorNormal * 3.1f;
        //     whatIsHangingFromTheRope.position = newPos;

        //     //currentGrabbable.IsGrabbable = false;

        // } else if (currentDistance > 3f && !currentGrabbable.IsGrabbed) {
        //     currentGrabbable.IsPhantom = false;
        //     whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;

        // } else {
        //     currentGrabbable.IsPhantom = false;
        // }


        // RopeSection lastRopeSection = allRopeSections[allRopeSections.Count - 1];
        // lastRopeSection.pos = whatIsHangingFromTheRope.position;
        // allRopeSections[allRopeSections.Count - 1] = lastRopeSection;

        //whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;

        // if (currentLength() > maxLength) {
        //     //Debug.Log("tooo looong");
        //     whatIsHangingFromTheRope.position = lastSectionPrevPos;
        // }
    }

    //Make sure the rope sections have the correct lengths
    private void ImplementMaximumStretch()
    {   
        // we calculate it for all but the last section, because we don't want that the lineRenderer
        // stops before reaching the whatIsHangingFromTheRope
        //int count = allRopeSections.Count - 1;
        // if (currentGrabbable.IsGrabbed) {
        //     count =- 1;
        // }
        for (int i = 0; i < allRopeSections.Count - 1; i++)
        {
            RopeSection topSection = allRopeSections[i];

            RopeSection bottomSection = allRopeSections[i + 1];

            //The distance between the sections
            float dist = (topSection.pos - bottomSection.pos).magnitude;

            //What's the stretch/compression
            float distError = Mathf.Abs(dist - sectionLength);

            Vector3 changeDir = Vector3.zero;

            //Compress this sections
            if (dist > sectionLength)
            {
                changeDir = (topSection.pos - bottomSection.pos).normalized;
            }
            //Extend this section
            else if (dist < sectionLength)
            {
                changeDir = (bottomSection.pos - topSection.pos).normalized;
            }
            //Do nothing
            else
            {
                continue;
            }


            Vector3 change = changeDir * distError;

            if (i != 0)
            {
                bottomSection.pos += change * 0.5f;

                allRopeSections[i + 1] = bottomSection;

                topSection.pos -= change * 0.5f;

                allRopeSections[i] = topSection;
            }
            //Because the rope is connected to something
            else
            {
                bottomSection.pos += change;

                allRopeSections[i + 1] = bottomSection;
            }
        }
    }

    //Display the rope with a line renderer
    private void DisplayRope()
    {
        float ropeWidth = 0.2f;

        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;

        //An array with all rope section positions
        Vector3[] positions = new Vector3[allRopeSections.Count];

        for (int i = 0; i < allRopeSections.Count; i++)
        {
            positions[i] = allRopeSections[i].pos;
        }

        lineRenderer.positionCount = positions.Length;

        lineRenderer.SetPositions(positions);
    }

    // returns current rope length
    private float currentLength() {
        float currentLength = 0f;
        for (int i = 0; i < allRopeSections.Count; i++) {
            if (i > 0) {
                currentLength += Vector3.Distance(allRopeSections[i - 1].pos, allRopeSections[i].pos);
            }
        }
        return currentLength;
    }

    //A struct that will hold information about each rope section
    public struct RopeSection
    {
        public Vector3 pos;
        public Vector3 oldPos;

        //To write RopeSection.zero
        public static readonly RopeSection zero = new RopeSection(Vector3.zero);

        public RopeSection(Vector3 pos)
        {
            this.pos = pos;

            this.oldPos = pos;
        }
    }
  }
}