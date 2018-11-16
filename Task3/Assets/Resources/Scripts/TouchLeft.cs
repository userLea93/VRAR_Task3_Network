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

    }
    private void Update()
    {
        if (vive)
        {
            if (grabPinchAction.GetStateUp(handType))
            {
                Debug.Log("Button released");
                if (!viveGrabScript)
                {
                    loadScript();
                }
                if (viveGrabScript)
                    viveGrabScript.setLeftTriggerDown(false);
            }
            if (grabPinchAction.GetLastStateDown(handType))
            {
                Debug.Log("Button down");
                if (!viveGrabScript)
                {
                    loadScript();
                }
                if (viveGrabScript) viveGrabScript.setLeftTriggerDown(true);
            }
        }
        
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
                    //Debug.Log("Touch left");
                    AuthorityManager am = other.gameObject.GetComponent<AuthorityManager>();
                    leapGrabScript.touchLeftDetected(am);
                }
            }
            else if(vive)
            {
                if (!viveGrabScript)
                {
                    loadScript();
                }
                if (viveGrabScript)
                {
                    AuthorityManager am = other.gameObject.GetComponent<AuthorityManager>();
                    viveGrabScript.setAuthorityManagerLeftHand(am);
                    viveGrabScript.setLeftHandTouching(true);
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
                    //Debug.Log("Release left");
                    leapGrabScript.touchLeftRelease();
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
    /*
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
                    viveGrabScript.setLeftHandTouching(true);
                    
                }
                else
                {
                    viveGrabScript.setLeftHandTouching(false);
                }
            }
        }
    }
    */

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
