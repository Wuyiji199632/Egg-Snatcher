using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(NetworkObject))]
public class PlayerFillManager : NetworkBehaviour,IGameStateListener
{

    [Header("Elements")]
    public PlayerFill[] players;

    private bool canUpatePlayers = false;

    [Header("Settings")]
    [SerializeField] private float fillStep = 0.1f;

    [Header("Actions")]
    public static Action<ulong> onPlayerEmpty;
    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState != GameState.Game)
        {
            return;
        }

        if (!IsServer)
        {
            return;
        }

        StartUpdatingPlayers();
    }

    private void StartUpdatingPlayers()
    {
        

        canUpatePlayers = true;

    }

    private void StorePlayers()
    {
        players = FindObjectsByType<PlayerFill>(FindObjectsSortMode.None);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canUpatePlayers)
            return;

        UpdatePlayersRpc(fillStep);
    }
    [Rpc(SendTo.Everyone)]
    private void UpdatePlayersRpc(float fillStep)
    {
        if (players == null || players.Length < 2)
            StorePlayers();
        foreach (var player in players)
        {

            if (player.UpdateFill(fillStep))
            {
                PlayerIsEmpty(player);
            }
        }
    }

    private void PlayerIsEmpty(PlayerFill player)
    {
         
        if(!IsServer)
            return;

        canUpatePlayers = false;

        onPlayerEmpty?.Invoke(player.GetComponent<NetworkObject>().OwnerClientId);
    }
}


