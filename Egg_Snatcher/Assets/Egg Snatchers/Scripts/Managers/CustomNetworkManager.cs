using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CustomNetworkManager : MonoBehaviour
{
    [SerializeField]
    private NetworkManager networkManager;


    [SerializeField]
    private List<Transform> spawnPoints;

    private void Start()
    {
        // Register events for Host and Client start
        networkManager.OnServerStarted += OnHostStarted;
        networkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        // Unregister events to avoid memory leaks
        networkManager.OnServerStarted -= OnHostStarted;
        networkManager.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnHostStarted()
    {
        // When host is started, spawn and move the host's player
        MovePlayerToSpawnPoint(NetworkManager.Singleton.LocalClientId);
    }

    private void OnClientConnected(ulong clientId)
    {
        // When a client connects, move them to a spawn point
        if (NetworkManager.Singleton.IsServer)
        {
            MovePlayerToSpawnPoint(clientId);
        }
    }

    private void MovePlayerToSpawnPoint(ulong clientId)
    {
        // Find the player object by clientId
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            Transform spawnPoint = ChooseSpawnPoint();
            if (spawnPoint != null)
            {
                client.PlayerObject.transform.position = spawnPoint.position;
                client.PlayerObject.transform.rotation = spawnPoint.rotation;
            }
        }
    }

    public Transform ChooseSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

}
