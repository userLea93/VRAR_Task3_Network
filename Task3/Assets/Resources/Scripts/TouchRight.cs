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
        if (leap)
        {
            if (other.gameObject.tag == "shared")
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
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (leap)
        {
            if (other.gameObject.tag == "shared")
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
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (vive)
        {
            if (!viveGrabScript)
            {
                loadScript();
            }

            if (viveGrabScript)
            {
                if (other.gameObject.tag == "shared")
                {
                    viveGrabScript.setRightHandTouching(true);
                    if (grabPinchAction.GetStateDown(handType))
                    {
                        Debug.Log("Right hand pressed");
                        viveGrabScript.setRightTriggerDown(true);
                    }
                    else
                    {
                        viveGrabScript.setRightTriggerDown(false);
                    }
                }
                else
                {
                    viveGrabScript.setRightHandTouching(false);
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
