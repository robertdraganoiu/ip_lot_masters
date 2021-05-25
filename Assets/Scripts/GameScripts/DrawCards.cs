using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class DrawCards : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public GameManager GameManager;
    public GameObject gm;
    int counter = 0;

    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        
        if (GameManager.GameState == "Ready?")
        {
            // check if pressed button already
            if(PlayerManager.isReady)
            {
                return;
            }
            Debug.Log("Player " + PlayerManager.id + " is ready.");
            PlayerManager.isReady = true;
            PlayerManager.CmdDealCards();
            startGame();

            // start game if both ready
            Debug.Log("There are " + GameManager.playersReady + " players ready before.");
            GameManager.playersReady++;

            Debug.Log("There are " + GameManager.playersReady + " players ready.");
            if (GameManager.playersReady == 2)
            {
                Debug.Log("Both players are ready.");
            }
        }
        else if (GameManager.GameState == "Playing")
        {
            PlayerManager.currentMana = PlayerManager.playerMana;
            PlayerManager.hasIncreasedMana = false;
            if (!isClientOnly)
            {
                counter++;
                if (counter == 2)
                {
                    counter = 0;
                } else
                {
                    PlayerManager.DrawCard();
                }  
            } 
            else
            {
                Debug.Log("Intra an functie?");
                PlayerManager.DrawCard();
            }
            foreach(GameObject card in PlayerManager.playerBattle)
            {
                card.GetComponent<Card>().isTapped = false;
                card.GetComponent<Outline>().enabled = false;
            }
            foreach (GameObject card in PlayerManager.enemyBattle)
            {
                card.GetComponent<Card>().isTapped = false;
                card.GetComponent<Outline>().enabled = false;
            }
            EndTurn();
            
        }
    }

    void startGame()
    {
        GameManager.setGameState("Playing");
    }

    void EndTurn()
    {
        GameManager.currentPlayerTurn = 1 - GameManager.currentPlayerTurn;
        Debug.Log("Current turn id is: " + GameManager.currentPlayerTurn);
    }

}
