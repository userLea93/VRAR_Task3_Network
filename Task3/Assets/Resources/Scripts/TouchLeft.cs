using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Valve.VR;

// TODO: this script CAN be used to detect the events of a left networked hand touching a shared object
// fill in the implementation and communicate touching events to either LeapGrab and ViveGrab by setting the rightHandTouching variable
// ALTERNATIVELY, implement the verification of the grabbing conditions in a way  your prefer
// TO REMEMBER: only the localPlayer (networked hands belonging to the localPlayer) should be able to "touch" shared objects

public class TouchLeft : MonoBehaviour {

    [SteamVR_DefaultAction("GrabPinch")]
    public SteamVR_Action_Boolean grabPinchAction;

    public SteamVR_Input_Sources handType;

    public bool vive;
    public bool leap;

    LeapGrab leapGrabScript;
    ViveGrab viveGrabScript;

    void Start()
    {
        if (leap)
        {
            Debug.Log("Script loading...");
            leapGrabScript = GetComponentInParent<LeapGrab>();
            if (leapGrabScript)
                Debug.Log("Success!");
        }
        else if (vive)
        {
            viveGrabScript = GetComponentInParent<ViveGrab>();
        }

    }

    void OnEnable()
    {
        if (leap)
        {
            Debug.Log("Script loading...");
            leapGrabScript = GetComponentInParent<LeapGrab>();
            if (leapGrabScript)
                Debug.Log("Success!");
        }
        else if (vive)
        {
            viveGrabScript = GetComponentInParent<ViveGrab>();
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "shared")
        {
            if (leap)
            {
                Debug.Log("Touch left");

                AuthorityManager am = other.gameObject.GetComponent<AuthorityManager>();
                leapGrabScript.touchLeftDetected(am);
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "shared")
        {
            if (leap)
            {
                leapGrabScript.touchLeftRelease();
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (vive)
        { 
            if (other.gameObject.tag == "shared")
            {
                viveGrabScript.setLeftHandTouching(true);
                if (grabPinchAction.GetStateDown(handType))
                {
                    Debug.Log("Left hand pressed");
                    viveGrabScript.setLeftTriggerDown(true);
                }
                else
                {
                    viveGrabScript.setLeftTriggerDown(false);
                }
            }
            else
            {
                viveGrabScript.setLeftHandTouching(false);
            }
        }
    }

}
