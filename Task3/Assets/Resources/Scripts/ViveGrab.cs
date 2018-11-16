using UnityEngine;
using System.Collections;
using Valve.VR;

// This script defines conditions that are necessary for the Vive player to grab a shared object
// TODO: values of these four boolean variables can be changed either directly here or through other components
// AuthorityManager of a shared object should be notifyed from this script

public class ViveGrab : MonoBehaviour
{

    AuthorityManager amLeftHand; // to communicate the fulfillment of grabbing conditions
    AuthorityManager amRightHand; // to communicate the fulfillment of grabbing conditions

    [SteamVR_DefaultAction("GrabPinch")]
    public SteamVR_Action_Boolean grabPinchAction;
    
    private SteamVR_Input_Sources handTypeLeft;
    private SteamVR_Input_Sources handTypeRight;
    

    // conditions for the object control here
    bool leftHandTouching = false;
    bool rightHandTouching = false;
    bool leftTriggerDown = false;
    bool rightTriggerDown = false;

    // Use this for initialization
    void Start()
    {
        SteamVR_Behaviour_Pose[] poseList = GetComponentsInChildren<SteamVR_Behaviour_Pose>();
        foreach (SteamVR_Behaviour_Pose pose in poseList)
        {
            if (pose.gameObject.tag == "left")
            {
                handTypeLeft = pose.inputSource;
            }
            else if(pose.gameObject.tag == "right")
            {
                handTypeRight= pose.inputSource;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        touchDetection(handTypeLeft, true);
        touchDetection(handTypeRight, false);
        if (leftHandTouching && rightHandTouching && leftTriggerDown && rightTriggerDown)
        {
            // notify AuthorityManager that grab conditions are fulfilled
            //am.grabbedByPlayer = true;
            Debug.Log("!!!!!Box GRABBED BY VIVE");
            if (amLeftHand != null && amLeftHand.netId == amRightHand.netId)
            {
                amLeftHand.grabbedByPlayer = true;
            }
        }
        else
        {
            //am.grabbedByPlayer = false;
            // grab conditions are not fulfilled
            if (amLeftHand != null)
            {
                amLeftHand.grabbedByPlayer = false;
            }
            if (amRightHand != null)
            {
                amRightHand.grabbedByPlayer = false;
            }
        }

    }
    private void touchDetection(SteamVR_Input_Sources handType, bool isLeft)
    {
        if (grabPinchAction.GetStateUp(handType))
        {
            Debug.Log("Button released");
            if (isLeft) setLeftTriggerDown(false);
            else setRightTriggerDown(false);
        }
        if (grabPinchAction.GetLastStateDown(handType))
        {
            Debug.Log("Button down");
            if (isLeft) setLeftTriggerDown(true);
            else setRightTriggerDown(true);
        }
    }

    public void setLeftHandTouching(bool set)
    {
        leftHandTouching = set;
    }
    public void setRightHandTouching(bool set)
    {
        rightHandTouching = set;
    }
    public void setLeftTriggerDown(bool set)
    {
        leftTriggerDown = set;
    }
    public void setRightTriggerDown(bool set)
    {
        rightTriggerDown = set;
    }

    public void setAuthorityManagerLeftHand(AuthorityManager authorityManager)
    {
        amLeftHand = authorityManager;
    }
    public void setAuthorityManagerRightHand(AuthorityManager authorityManager)
    {
        amRightHand = authorityManager;
    }
}
