using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// This script defines conditions that are necessary for the Leap player to grab a shared object
// TODO: values of these four boolean variables can be changed either directly here or through other components
// AuthorityManager of a shared object should be notifyed from this script

public class LeapGrab : MonoBehaviour {

    AuthorityManager am;

    // conditions for the object control here
    bool leftHandTouching = false;
    bool rightHandTouching = false;
    bool leftPinch = false;
    bool rightPinch = false;
    

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        Debug.Log("Left Pinch: " + leftPinch);
        Debug.Log("Right Pinch: " + rightPinch);

        if (leftHandTouching && rightHandTouching && leftPinch && rightPinch)
        {
            Debug.Log("Leap grab detected!!!");

            // notify AuthorityManager that grab conditions are fulfilled
            if (am)
            {
                am.grabbedByPlayer = true;
            }
        }
        else
        {
            // grab conditions are not fulfilled
            if (am)
            {
                am.grabbedByPlayer = false;
            }
        }
    }

    
    public void leftPincheDetected()
    {
        leftPinch = true;
        Debug.Log("Left Pinch!");
    }

    public void rightPincheDetected()
    {
        rightPinch = true;
        Debug.Log("Right Pinch!");
    }

    public void leftPinchReleased()
    {
        //leftPinch = false;
    }

    public void rightPinchReleased()
    {
        //rightPinch = false;
    }

    public void touchLeftDetected(AuthorityManager authorityManager)
    {
        //if (rightHandTouching && am)
        //{
        //    if (am.netId != authorityManager.netId) // if left and right hand are touching different objects
        //    {
        //        leftHandTouching = false;
        //        return;
        //    }
        //}
        //else
        //{
            am = authorityManager;
        //}

        Debug.Log("Touch left!");
        leftHandTouching = true;

    }

    public void touchRightDetected(AuthorityManager authorityManager)
    {
        //if (leftHandTouching && am)
        //{
        //    if (am.netId != authorityManager.netId) // if left and right hand are touching different objects
        //    {
        //        rightHandTouching = true;
        //        return;
        //    }
        //}
        //else
        //{
            am = authorityManager;
        //}

        Debug.Log("Touch right!");
        rightHandTouching = true;

    }

    public void touchRightRelease()
    {
        //rightHandTouching = false;
    }

    public void touchLeftRelease()
    {
       // leftHandTouching = false;
    }

}
