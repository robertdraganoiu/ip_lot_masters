using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public UIManager UIManager;
    public string GameState = "Ready?";
    [SyncVar]
    public int currentPlayerTurn = 0;
    [SyncVar]
    public int playersReady = 0;

    void Start()
    {
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        UIManager.UpdateButtonText(GameState);
    }

    public void setGameState(string state)
    {
        GameState = state;
        if (state.Equals("Playing"))
        {
            UIManager.UpdateButtonText("End Turn");
        }
    }

    public void CardPlayed()
    {
        // TODO add logic when card is played
    }
}
