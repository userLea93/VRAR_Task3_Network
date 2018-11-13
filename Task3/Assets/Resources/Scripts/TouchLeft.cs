using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// TODO: this script CAN be used to detect the events of a left networked hand touching a shared object
// fill in the implementation and communicate touching events to either LeapGrab and ViveGrab by setting the rightHandTouching variable
// ALTERNATIVELY, implement the verification of the grabbing conditions in a way  your prefer
// TO REMEMBER: only the localPlayer (networked hands belonging to the localPlayer) should be able to "touch" shared objects

public class TouchLeft : MonoBehaviour {

    public bool vive;
    public bool leap;

    LeapGrab leapGrabScript;
    ViveGrab viveGrabScript;

    void Start()
    {
        if (leap)
        {
            leapGrabScript = GetComponentInParent<LeapGrab>();
            Debug.Log("LeapGrabScript: " + leapGrabScript.gameObject.name);
        }
        else if (vive)
        {
            viveGrabScript = GetComponentInParent<ViveGrab>();
            Debug.Log("ViveGrabScript: " + viveGrabScript.gameObject.name);

        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "shared")
        {
            if (leap)
            {
                AuthorityManager am = other.gameObject.GetComponent<AuthorityManager>();
                leapGrabScript.touchLeftDetected(am);
            }

        }

    }

}
