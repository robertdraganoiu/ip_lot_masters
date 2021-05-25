using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardConnectionNetwork : NetworkBehaviour
{
    public PlayerManager playerManager;
    public void Start()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();
    }
}
