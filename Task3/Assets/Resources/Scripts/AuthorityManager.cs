using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


// TODO: this script should manage authority for a shared object
public class AuthorityManager : NetworkBehaviour {

    
    NetworkIdentity netID; // NetworkIdentity component attached to this game object

    // these variables should be set up on a client
    //**************************************************************************************************
    Actor localActor; // Actor that is steering this player 

    private bool requestProcessed = true; // on start, there are no requests therefor we don't have to wait for them
    private bool grabbed = false; // if this is true client authority for the object should be requested
    public bool grabbedByPlayer // private "grabbed" field can be accessed from other scripts through grabbedByPlayer
    {
        get { return grabbed; }
        set { grabbed = value; }
    }

    OnGrabbedBehaviour onb; // component defining the behaviour of this GO when it is grabbed by a player
                            // this component can implement different functionality for different GO´s


    //***************************************************************************************************

    // these variables should be set up on the server

    // TODO: implement a mechanism for storing consequent authority requests from different clients
    // e.g. manage a situation where a client requests authority over an object that is currently being manipulated by another client

    //*****************************************************************************************************

    // TODO: avoid sending two or more consecutive RemoveClientAuthority or AssignClientAUthority commands for the same client and shared object
    // a mechanism preventing such situations can be implemented either on the client or on the server

    // Use this for initialization
    void Start () {

        netID = this.gameObject.GetComponent<NetworkIdentity>();
        onb = this.gameObject.AddComponent<OnGrabbedBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {
        
 
        //if (isClient && netID.hasAuthority)
        //{
        //    Debug.Log("Client has box authority");
        //}

        if (isClient)
        {
            if (requestProcessed)
            {
                if (grabbed && !netID.hasAuthority) // grab conditions are fulfilled but actor does not have authority -> request!
                {
                    Debug.Log("REQUEST authority of " + netID.ToString());
                    requestProcessed = false;
                    localActor.RequestObjectAuthority(netID);
                }
                else if (!grabbed && netID.hasAuthority)
                {
                    requestProcessed = false;
                    localActor.ReturnObjectAuthority(netID);
                }
            }

            
        }
    }

    // assign localActor here
    public void AssignActor(Actor actor)
    {
        localActor = actor;
    }

    // should only be called on server (by an Actor)
    // assign the authority over this game object to a client with NetworkConnection conn
    public void AssignClientAuthority(NetworkConnection conn)
    {
        Debug.Log("Assign Authority!");
        netID.AssignClientAuthority(conn);
        //grabbed = true; // should be true already
    }

    // should only be called on server (by an Actor)
    // remove the authority over this game object from a client with NetworkConnection conn
    public void RemoveClientAuthority(NetworkConnection conn)
    {
        Debug.Log("Remove Authority!");
        netID.RemoveClientAuthority(conn);

        //grabbed = false;
    }

    [TargetRpc]
    public void TargetAuthorityAssigned(NetworkConnection connection)
    {
        Debug.Log("Get authority from host.");
        requestProcessed = true;

        onb.OnGrabbed(localActor);
        
    }
    [TargetRpc]
    public void TargetAuthorityRemoved(NetworkConnection connection)
    {
        Debug.Log("Remove Authority from Host.");
        requestProcessed = true;
        onb.OnReleased();
        
    }
    
    [TargetRpc]
    public void TargetAuthorityDeclined(NetworkConnection connection)
    {
        Debug.Log("Declined Authority from Host. Please wait");
        requestProcessed = true;
    }
}
