﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Actor : NetworkBehaviour {

    public Character character;
    public new Transform transform;

    [SyncVar]
    private string prefabName = "";

    //this part is for object sharing
    //*******************************
    List<NetworkIdentity> sharedObjects; // shared objects on the server or localActor
    //*******************************


    protected virtual void Awake()
    {
        transform = base.transform;
    }

    // Use this for initialization
    void Start () {

        if (isServer || isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                // Inform the local player about his new character
                LocalPlayerController.Singleton.SetActor(this);
                CmdInitialize(prefabName);
            }

            sharedObjects = new List<NetworkIdentity>();

            //this part is for object sharing
            //*******************************
            if (isServer)
            {
                // find objects that can be manipulated 
                // TIPP : you can use a specific tag for all GO's that can be manipulated by players
                GameObject[] sharedObjsArr = GameObject.FindGameObjectsWithTag("shared");
                for (int i = 0; i < sharedObjsArr.Length; i++)
                {
                    sharedObjects.Add(sharedObjsArr[i].GetComponent<NetworkIdentity>());
                }
            }
            if (isLocalPlayer) 
            {
                // find objects that can be manipulated 
                // assign this Actor to the localActor field of the AuthorityManager component of each shared object
                GameObject[] sharedObjsArr = GameObject.FindGameObjectsWithTag("shared");
                for (int i = 0; i < sharedObjsArr.Length; i++)
                {
                    Rigidbody rb = sharedObjsArr[i].GetComponent<Rigidbody>();
                    if (rb)
                    {
                        rb.isKinematic = true;
                    }
                    sharedObjsArr[i].GetComponent<AuthorityManager>().AssignActor(this);
                    sharedObjects.Add(sharedObjsArr[i].GetComponent<NetworkIdentity>());
                }
            }
            //*******************************

        }
        else
        {
            // Initialize on startup
            Initialize(prefabName);
        }

    }

    public void Update()
    {
        

    }

    /// <summary>
    /// Updates the actor position and rotation.
    /// This function should be called only by the <see cref="LocalPlayerController"/>.
    /// </summary>
    public void UpdateActorLeft(Vector3 leftPos, Quaternion leftRot) //runs only on LocalPlayer!
    {
        if (character != null)
        {
            character.UpdateCharacterLeft(leftPos, leftRot);
        }
    }

    /// <summary>
    /// Updates the actor position and rotation.
    /// This function should be called only by the <see cref="PlayerController"/>.
    /// </summary>
    public void UpdateActorRight(Vector3 rightPos, Quaternion rightRot) //runs only on LocalPlayer!
    {
        if (character != null)
        {
            character.UpdateCharacterRight(rightPos, rightRot);
        }
    }

    public void SetRightCharacterActive(bool active)
    {
        character.SetRightActive(active);
    }

    public void SetLeftCharacterActive(bool active)
    {
        character.SetLeftActive(active);
    }

    /// <summary>
    /// Initialize the player locally.
    /// </summary>
    /// <param name="prefab">Prefab character name.</param>
    public void Initialize(string prefab)
    {
        prefabName = prefab;
        name = name.Replace("(Clone)", "");

    }

    /// <summary>
    /// Spawns the character of actor on all clients.
    /// This runs on server only.
    /// </summary>
    /// <param name="prefab">Prefab name of the character.</param>
    private void SpawnCharacter(string prefab)
    {
        // Spawn character
        GameObject modelPrefab = Resources.Load("Prefabs/" + prefab) as GameObject;
        GameObject model = (GameObject)Instantiate(modelPrefab, transform.position, transform.rotation) as GameObject;
        NetworkServer.Spawn(model);

        // Attach character to player
        AttachCharacter(model.GetComponent<Character>());
    }

    /// <summary>
    /// Initializes the character on server to inform all clients. 
    /// This command calls the Initialize() method and spawns the character.
    /// </summary>
    [Command]
    public void CmdInitialize(string prefab)
    {
        if (prefab.Length > 0)
        {
            CreateCharacter(prefab);
        }
    }

    /// <summary>
    /// Creates the character and initializes on server.
    /// </summary>
    /// <param name="prefab">The character prefab name.</param>
    [Server]
    public void CreateCharacter(string prefab)
    {
        SpawnCharacter(prefab);
        Initialize(prefab);
    }

    /// <summary>
    /// Main routine to attach the character to this actor
    /// This runs only on Server.
    /// </summary>
    /// <param name="newCharacter">New character to attach.</param>
    [Server]
    public void AttachCharacter(Character newCharacter)
    {
        newCharacter.AttachToActor(netId);
    }


    //this part is for object sharing
    // fill in the implementation
    //*******************************

    // should only be run on localPlayer (by the AuthorityManager of a shared object)
    // ask the server for the authority over an object with NetworkIdentity netID
    public void RequestObjectAuthority(NetworkIdentity netID)
    {
        if (isLocalPlayer)
        {
            Debug.Log("Request authority...");
            CmdAssignObjectAuthorityToClient(netID);
        }
    }

    // should only be run on localPlayer (by the AuthorityManager of a shared object)
    // ask the server to remove the authority over an object with NetworkIdentity netID
    public void ReturnObjectAuthority(NetworkIdentity netID)
    {
        if (isLocalPlayer)
        {
            Debug.Log("Remove authority...");
            CmdRemoveObjectAuthorityFromClient(netID);
        }  
    }
    Dictionary<NetworkIdentity, NetworkConnection> authorityRequestToProcess = new Dictionary<NetworkIdentity, NetworkConnection>();


    // run on the server
    // netID is NetworkIdentity of a shared object the authority if which should be passed to the client
    [Command]
    void CmdAssignObjectAuthorityToClient(NetworkIdentity netID)
    {

        Debug.Log("On Server : Start CmdAssignObjectAuthorityToClient");
        NetworkConnection otherOwner = netID.clientAuthorityOwner;


        if (otherOwner != null && otherOwner != connectionToClient)
        {
            Debug.Log("On Server : Other client has authority. Save request");
            //netID.gameObject.GetComponent<AuthorityManager>().RemoveClientAuthority(otherOwner);
            //netID.gameObject.GetComponent<AuthorityManager>().TargetAuthorityRemoved(otherOwner);
            Dictionary<NetworkIdentity, NetworkConnection> tempDict = new Dictionary<NetworkIdentity, NetworkConnection>();
            foreach (KeyValuePair<NetworkIdentity, NetworkConnection> pair in authorityRequestToProcess)
            {
                //Now you can access the key and value both separately from this attachStat as:
                if (pair.Value != connectionToClient) tempDict.Add(pair.Key, pair.Value);
            }
            authorityRequestToProcess = tempDict;
            if (!authorityRequestToProcess.ContainsKey(netID))
            {
                authorityRequestToProcess.Add(netID, connectionToClient);
            }
            netID.gameObject.GetComponent<AuthorityManager>().TargetAuthorityDeclined(connectionToClient);
        }
        else
        {
            Debug.Log("On Server : Client gets authority");
            Rigidbody rb = netID.gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = true;

            netID.gameObject.GetComponent<AuthorityManager>().AssignClientAuthority(connectionToClient);
            netID.gameObject.GetComponent<AuthorityManager>().TargetAuthorityAssigned(connectionToClient);
        }
        Debug.Log("On Server : End CmdAssignObjectAuthorityToClient");

    }

    // run on the server
    // netID is NetworkIdentity of a shared object the authority if which should be removed from the client
    [Command]
    void CmdRemoveObjectAuthorityFromClient(NetworkIdentity netID)
    {
        Debug.Log("On Server : Start CmdRemoveObjectAuthorityFromClient");
        NetworkConnection otherOwner = netID.clientAuthorityOwner;

        if (otherOwner != null && otherOwner == connectionToClient)
        {
            Debug.Log("On Server : Client has Objekt - Remove Authority from Client");

            netID.gameObject.GetComponent<AuthorityManager>().RemoveClientAuthority(connectionToClient);
            Rigidbody rb = netID.gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            netID.gameObject.GetComponent<AuthorityManager>().TargetAuthorityRemoved(connectionToClient);
            if (authorityRequestToProcess.ContainsKey(netID))
            {
                Debug.Log("Server: Other client is waiting - Assign Authority");

                rb = netID.gameObject.GetComponent<Rigidbody>();
                rb.isKinematic = true;

                netID.gameObject.GetComponent<AuthorityManager>().AssignClientAuthority(authorityRequestToProcess[netID]);
                netID.gameObject.GetComponent<AuthorityManager>().TargetAuthorityAssigned(authorityRequestToProcess[netID]);
                authorityRequestToProcess.Remove(netID);
            }
        }
        else 
        {
            Debug.Log("Server: Waiting Client wants to remove authority");
            Dictionary<NetworkIdentity, NetworkConnection> tempDict = new Dictionary<NetworkIdentity, NetworkConnection>();
            foreach (KeyValuePair<NetworkIdentity, NetworkConnection> pair in authorityRequestToProcess)
            {
                //Now you can access the key and value both separately from this attachStat as:
                if (pair.Value != connectionToClient) tempDict.Add(pair.Key, pair.Value);
            }
            authorityRequestToProcess = tempDict;
            netID.gameObject.GetComponent<AuthorityManager>().TargetAuthorityRemoved(connectionToClient);

        }
        Debug.Log("On Server : END CmdRemoveObjectAuthorityFromClient");


    }
    //*******************************
}
