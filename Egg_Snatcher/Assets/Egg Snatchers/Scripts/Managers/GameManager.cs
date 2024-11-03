using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
public enum GameState
{
    Waiting,Game, Win, Lose
}
[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;



    public GameState gameState;
    private void Awake()
    {
        gameState = GameState.Waiting;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        NetworkManager.OnClientConnectedCallback += ClientConnectedCallback;
        PlayerFillManager.onPlayerEmpty += PlayerEmptyCallback;
    }
    //public override void OnDestroy()
    //{
    //    base.OnDestroy();
    //    NetworkManager.OnClientConnectedCallback -= ClientConnectedCallback;
    //    PlayerFillManager.onPlayerEmpty -= PlayerEmptyCallback;
    //}

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkManager.OnClientConnectedCallback -= ClientConnectedCallback;
        PlayerFillManager.onPlayerEmpty -= PlayerEmptyCallback;
    }
    private void PlayerEmptyCallback(ulong losingPlayerId)
    {
        if(gameState!= GameState.Game)
        {
            return;
        }
        GameEndedRpc(losingPlayerId);
    }

    [Rpc(SendTo.Everyone)]
    private void GameEndedRpc(ulong losingPlayerId)
    {
        ulong localPlayerId = NetworkManager.SpawnManager.GetLocalPlayerObject().OwnerClientId;

        if (losingPlayerId == localPlayerId)
        {
            SetGameState(GameState.Lose);
        }
        else
        {
            SetGameState(GameState.Win);
        }
    }

    private void ClientConnectedCallback(ulong obj)
    {
        if (!IsServer) return;
        if (NetworkManager.Singleton.ConnectedClientsList.Count < 2)
            return;

        StartGameRpc();
    }
    [Rpc(SendTo.Everyone)]
    private void StartGameRpc()
    {
        SetGameState(GameState.Game);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;

        IEnumerable<IGameStateListener> gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameStateListener>();

        foreach (var gameStateListener in gameStateListeners)
        {
            gameStateListener.GameStateChangedCallback(gameState);
        }

        Debug.Log("Net Game State: " + gameState);
    }

    public bool IsGameState()
    {
        return gameState == GameState.Game;
    }
}

public interface IGameStateListener
{
    void GameStateChangedCallback(GameState gameState);
}