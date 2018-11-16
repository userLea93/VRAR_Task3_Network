using UnityEngine;
using System.Collections;
using Valve.VR;

// TODO: this script CAN be used to detect the events of a right networked hand touching a shared object
// fill in the implementation and communicate touching events to either LeapGrab and ViveGrab by setting the rightHandTouching variable
// ALTERNATIVELY, implement the verification of the grabbing conditions in a way  your prefer
// TO REMEMBER: only the localPlayer (networked hands belonging to the localPlayer) should be able to "touch" shared objects

public class TouchRight : MonoBehaviour {

    [SteamVR_DefaultAction("GrabPinch")]
    public SteamVR_Action_Boolean grabPinchAction;

    public SteamVR_Input_Sources handType;

    // the implementation of a touch condition might be different for Vive and Leap 
    public bool vive;
    public bool leap;

    LeapGrab leapGrabScript;
    ViveGrab viveGrabScript;

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "shared")
        {
            if (leap)
            {
                if (!leapGrabScript)
                {
                    loadScript();
                }

                if (leapGrabScript)
                {
                    //Debug.Log("Touch right");

                    AuthorityManager am = other.gameObject.GetComponent<AuthorityManager>();
                    leapGrabScript.touchRightDetected(am);
                }
            }
            else if (vive)
            {
                if (!viveGrabScript)
                {
                    loadScript();
                }
                if (viveGrabScript)
                {
                    AuthorityManager am = other.gameObject.GetComponent<AuthorityManager>();
                    viveGrabScript.setAuthorityManagerRightHand(am);
                    viveGrabScript.setRightHandTouching(true);
                }
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "shared")
        {
            if (leap)
            {
                if (!leapGrabScript)
                {
                    loadScript();
                }

                if (leapGrabScript)
                {
                    //Debug.Log("Release right");
                    leapGrabScript.touchRightRelease();
                }
            }
            else if (vive)
            {
                if (!viveGrabScript)
                {
                    loadScript();
                }
                if (viveGrabScript)
                {
                    viveGrabScript.setLeftHandTouching(false);
                }
            }
        }
    }


    private void loadScript()
    {
        if (leap)
        {
            leapGrabScript = GetComponentInParent<LeapGrab>();
        }
        else if (vive)
        {
            viveGrabScript = GetComponentInParent<ViveGrab>();
        }
    }
}
