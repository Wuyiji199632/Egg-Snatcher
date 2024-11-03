using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraManager : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private CinemachineTargetGroup targetGroup;

    [SerializeField]
    private bool configured = false;

    [SerializeField]
    private List<PlayerController> playerControllers;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.OnServerStarted += ServerConnectedCallback;
       
    }

    private void ServerConnectedCallback()
    {
        if (!IsServer) return;
        NetworkManager.OnClientConnectedCallback += ClientConnectedCallback;
    }

    //public override void OnDestroy()
    //{
    //    base.OnDestroy();
    //    if (!IsServer) return;
    //    NetworkManager.OnClientConnectedCallback -= ClientConnectedCallback;
    //}

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsServer) return;
        NetworkManager.OnClientConnectedCallback -= ClientConnectedCallback;
        NetworkManager.OnServerStarted -= ServerConnectedCallback;
    }
    private void ClientConnectedCallback(ulong clientID)
    {
        int playerCount=NetworkManager.Singleton.ConnectedClients.Count;

        if (playerCount < 2)
            return;
        StorePlayersRpc();


        UpdateCameraTargetGroupRpc();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (configured) return;

    //    if (playerControllers.Count < 2)
    //    {
    //        StorePlayers();return;
    //    }

    //    UpdateCameraTargetGroup();
    //}
    [Rpc(SendTo.Everyone)]
    private void UpdateCameraTargetGroupRpc()
    {
        configured = true;

        foreach (var playerController in playerControllers)
        {

            float weight = 1;

            if (playerController.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                weight = 10;
            }

            targetGroup.AddMember(playerController.transform, weight, 2);
        }
    }
    [Rpc(SendTo.Everyone)]
    private void StorePlayersRpc()
    {
        PlayerController[] playerControllersArray = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);


        //if (playerControllersArray.Length < 2)
        //    return;

        playerControllers = new List<PlayerController>(playerControllersArray);

      

    }
}
