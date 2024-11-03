using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class EggManager : NetworkBehaviour
{

    [Header("Elements")]
    [SerializeField]
    private GameObject eggPrefab;

    [SerializeField]
    private Transform[] spawnPositions;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.OnServerStarted += ServerStartedCallback;
        
    }


    //public override void OnDestroy()
    //{
    //    NetworkManager.OnServerStarted -= ServerStartedCallback;
    //}
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkManager.OnServerStarted -= ServerStartedCallback;
    }
    private void ServerStartedCallback()
    {
        if (!IsServer) return;

        SpawnEggRpc();
    }
    [Rpc(SendTo.Everyone)]
    private void SpawnEggRpc()
    {
        
        Vector3 spawnPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position;

        var eggInstance=Instantiate(eggPrefab, spawnPosition, Quaternion.identity);

        eggInstance.GetComponent<NetworkObject>().Spawn();
    }

    
}
