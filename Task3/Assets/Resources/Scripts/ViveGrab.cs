using UnityEngine;
using System.Collections;

// This script defines conditions that are necessary for the Vive player to grab a shared object
// TODO: values of these four boolean variables can be changed either directly here or through other components
// AuthorityManager of a shared object should be notifyed from this script

public class ViveGrab : MonoBehaviour
{

    AuthorityManager amLeftHand; // to communicate the fulfillment of grabbing conditions
    AuthorityManager amRightHand; // to communicate the fulfillment of grabbing conditions



    // conditions for the object control here
    bool leftHandTouching = false;
    bool rightHandTouching = false;
    bool leftTriggerDown = false;
    bool rightTriggerDown = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
            if (amLeftHand != null && amLeftHand.netId == amRightHand.netId)
            {
                amLeftHand.grabbedByPlayer = false;
            }
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
