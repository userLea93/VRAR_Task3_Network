using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// TODO: define the behaviour of a shared object when it is manipulated by a client

public class OnGrabbedBehaviour : MonoBehaviour
{


    bool grabbed;
    Actor owner;

    // Use this for initialization
    void Start()
    {
        grabbed = false;
    }

    // Update is called once per frame
    void Update()
    {

        // GO´s behaviour when it is in a grabbed state (owned by a client) should be defined here
        if (grabbed && owner)
        {
            this.gameObject.transform.position = (owner.character.left.position + owner.character.right.position) / 2.0f; // cause Warning: HandleTransform netId:1 (Box) is not for a valid player
        }
    }

    // called first time when the GO gets grabbed by a player
    public void OnGrabbed(Actor actor)
    {
        //Debug.Log("OnGrabbedBahavior entering ...");

        //Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        //rb.isKinematic = true;
        grabbed = true;
        owner = actor;
    }

    // called when the GO gets released by a player
    public void OnReleased()
    {
        //Debug.Log("OnGrabbedBahavior release!");

        //Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        //rb.isKinematic = false;
        grabbed = false;
        owner = null;
    }
}
