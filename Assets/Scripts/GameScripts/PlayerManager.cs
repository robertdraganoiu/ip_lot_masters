using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using JetBrains.Annotations;

public class PlayerManager : NetworkBehaviour
{

    public GameObject Azoth;
    public GameObject Diana;
    public GameObject Johnson;
    public GameObject Kor;
    public GameObject Orion;
    public GameObject Roland;
    public GameObject Scarlet;
    public GameObject Sidra;
    public GameObject Theros;
    public GameObject Thatch;
    public GameObject Ulgrim;
    public GameObject WuShang;

    public GameManager GameManager;
    public GameObject Card;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject PlayerAvatar;
    public GameObject EnemyAvatar;
    public GameObject PlayerSlot1;
    public GameObject PlayerSlot2;
    public GameObject PlayerSlot3;
    public GameObject PlayerSlot4;
    public GameObject PlayerSlot5;
    public GameObject EnemySlot1;
    public GameObject EnemySlot2;
    public GameObject EnemySlot3;
    public GameObject EnemySlot4;
    public GameObject EnemySlot5;
    public GameObject PlayerYard;
    public GameObject EnemyYard;
    public GameObject EnemyBattleZone;
    public GameObject PlayerBattleZone;
    public GameObject PlayerManaZone;
    public GameObject EnemyManaZone;
    public GameObject PlayerDeck;
    public GameObject EnemyDeck;
    public List<GameObject> PlayerShields = new List<GameObject>();
    public List<GameObject> EnemyShields = new List<GameObject>();

    private static int deckSize = 20;

    public int id = 1;
    public int playerMana = 0;
    public int currentMana = 0;
    public int enemyMana = 0;
    public int numOfShields = 4;
    private int curShieldIndex = 4;
    public bool shieldsDealt = false;
    public bool isReady = false;
    public bool hasIncreasedMana = false;
    public List<GameObject> myDeck = new List<GameObject>();
    public List<GameObject> enemyDeck = new List<GameObject>();

    public int CardsPlayed = 0;

    public List<GameObject> deck = new List<GameObject>();
    public List<GameObject> playerBattle = new List<GameObject>();
    public List<GameObject> enemyBattle = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        Debug.Log("Creating client.");

        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        PlayerAvatar = GameObject.Find("PlayerAvatar");
        EnemyAvatar = GameObject.Find("EnemyAvatar");
        PlayerYard = GameObject.Find("PlayerYard");
        EnemyYard = GameObject.Find("EnemyYard");
        EnemyBattleZone = GameObject.Find("EnemyBattlezone");
        PlayerBattleZone = GameObject.Find("PlayerBattlezone");
        PlayerManaZone = GameObject.Find("PlayerMana");
        EnemyManaZone = GameObject.Find("EnemyMana");
        PlayerDeck = GameObject.Find("PlayerDeck");
        EnemyDeck = GameObject.Find("EnemyDeck");
        // shields
        PlayerSlot1 = GameObject.Find("PlayerSlot1");
        PlayerSlot2 = GameObject.Find("PlayerSlot2");
        PlayerSlot3 = GameObject.Find("PlayerSlot3");
        PlayerSlot4 = GameObject.Find("PlayerSlot4");
        PlayerSlot5 = GameObject.Find("PlayerSlot5");
        EnemySlot1 = GameObject.Find("EnemySlot1");
        EnemySlot2 = GameObject.Find("EnemySlot2");
        EnemySlot3 = GameObject.Find("EnemySlot3");
        EnemySlot4 = GameObject.Find("EnemySlot4");
        EnemySlot5 = GameObject.Find("EnemySlot5");

        PlayerShields.Add(PlayerSlot1);
        PlayerShields.Add(PlayerSlot2);
        PlayerShields.Add(PlayerSlot3);
        PlayerShields.Add(PlayerSlot4);
        PlayerShields.Add(PlayerSlot5);
        EnemyShields.Add(EnemySlot1);
        EnemyShields.Add(EnemySlot2);
        EnemyShields.Add(EnemySlot3);
        EnemyShields.Add(EnemySlot4);
        EnemyShields.Add(EnemySlot5);

        if (isClientOnly)
        {
            id = 0;
            Debug.Log("Changed client id.");
        }
    }

    [Server]
    public override void OnStartServer()
    {
        Debug.Log("Created server.");
        id = 1;
        deck.Add(Azoth);
        deck.Add(Diana);
        deck.Add(Johnson);
        deck.Add(Kor);
        deck.Add(Orion);
        deck.Add(Roland);
        deck.Add(Scarlet);
        deck.Add(Sidra);
        deck.Add(Theros);
        deck.Add(Thatch);
        deck.Add(Ulgrim);
        deck.Add(WuShang);
        Debug.Log(deck.Count);
    }

    [Command]
    public void CmdDealCards()
    {
        // create deck with random cards
        for (int i = 0; i < deckSize; ++i)
        {
            GameObject card = Instantiate(deck[Random.Range(0, deck.Count)], new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(card, connectionToClient);
            myDeck.Add(card);
        }

        for (int i = 0; i < 5; i++)
        {
            RpcShowCard(myDeck[0], "Shields");
            myDeck.RemoveAt(0);
        }

        for (int i = 0; i < 5; i++)
        {
            RpcShowCard(myDeck[0], "Hand");
            myDeck.RemoveAt(0);
        }

        for (int i = 0; i < deckSize - 10; ++i)
        {
            RpcShowCard(myDeck[i], "Deck");
        }
    }

    public void DrawCard()
    {   
        if (myDeck.Count > 0)
        {
            CmdPlayCard(myDeck[0], "HandFromDeck");
            myDeck.RemoveAt(0);
        }
    }

    public void PlayCard(GameObject card, string zone)
    {
        Debug.Log(card == null);
        CmdPlayCard(card, zone);
    }

    [Command]
    void CmdPlayCard(GameObject card, string zone)
    {
        RpcShowCard(card, zone);
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        if (type == "Hand")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyArea.transform, false);
                card.GetComponent<Card>().Flip();
            }
        }
        else if (type == "HandFromDeck")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerArea.transform, false);
                card.GetComponent<Card>().Flip();
            }
            else
            {
                card.transform.SetParent(EnemyArea.transform, false);
            }
        }
        else if (type == "Shields")
        {
            CardsPlayed %= 5;
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerShields[CardsPlayed].transform, false);
                card.GetComponent<Card>().Flip();
            }
            if (!hasAuthority)
            {
                card.transform.SetParent(EnemyShields[CardsPlayed].transform, false);
                card.GetComponent<Card>().Flip();
            }
            CardsPlayed++;
        }
        else if (type == "Mana")
        {
            if (hasAuthority)
            {
                Debug.Log("Before: " + playerMana + " " + currentMana);
                playerMana++;
                currentMana++;
                Debug.Log("After: " + playerMana + " " + currentMana);
                card.transform.SetParent(PlayerManaZone.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyManaZone.transform, false);
                card.GetComponent<Card>().Flip();
            }
        }
        else if (type == "MyBattleZone")
        {
            if (hasAuthority)
            {
                playerBattle.Add(card);
                card.transform.SetParent(PlayerBattleZone.transform, false);
            }
            else
            {
                enemyBattle.Add(card);
                card.transform.SetParent(EnemyBattleZone.transform, false);
                card.GetComponent<Card>().Flip();
            }
        }
        else if (type == "Deck")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerDeck.transform, false);
                card.GetComponent<Card>().Flip();
            }
            else
            {
                enemyDeck.Add(card);
                card.transform.SetParent(EnemyDeck.transform, false);
                card.GetComponent<Card>().Flip();
            }
        }
    }

    public void AttackCard(GameObject attacker, GameObject defender)
    {
        if (!attacker.GetComponent<Card>().canAttackCreatures)
        {
            Debug.Log("[INFO] Attacker could not attack creature as it is not able.");
            attacker.transform.SetParent(PlayerBattleZone.transform, false);
            return;
        }
        if (attacker.GetComponent<Card>().isTapped)
        {
            Debug.Log("[INFO] Attacker could not attack creature as it is tapped.");
            attacker.transform.SetParent(PlayerBattleZone.transform, false);
            return;
        }
        attacker.GetComponent<Card>().isTapped = true;
        attacker.GetComponent<Outline>().enabled = true;
        defender.GetComponent<Card>().isTapped = true;
        defender.GetComponent<Outline>().enabled = true;
        CmdAttackCard(attacker, defender);
    }

    [Command]
    void CmdAttackCard(GameObject attacker, GameObject defender)
    {
        RpcAttackCard(attacker, defender);
    }

    [ClientRpc]
    void RpcAttackCard(GameObject attacker, GameObject defender)
    {
        attacker.GetComponent<Card>().onAttacking();
        int attPower = attacker.GetComponent<Card>().power;
        int defPower = defender.GetComponent<Card>().power;

        Debug.Log("[INFO] Attacker: " + attacker.GetComponent<Card>().name);
        Debug.Log("[INFO] Defender: " + defender.GetComponent<Card>().name);

        attacker.GetComponent<Card>().onFinishingAttack();

        if (attPower > defPower)
        {
            Debug.Log("[INFO] Attacker has greater power. Defender dies.");
            if (hasAuthority)
            {
                enemyBattle.Remove(defender);
                defender.transform.SetParent(EnemyYard.transform, false);
                attacker.transform.SetParent(PlayerBattleZone.transform, false);
            }
            else
            {
                playerBattle.Remove(defender);
                defender.transform.SetParent(PlayerYard.transform, false);
                attacker.transform.SetParent(EnemyBattleZone.transform, false);

            }
        }
        else if (attPower < defPower)
        {
            Debug.Log("[INFO] Defender has greater power. Attacker dies.");
            if (hasAuthority)
            {
                playerBattle.Remove(attacker);
                attacker.transform.SetParent(PlayerYard.transform, false);
            }
            else
            {
                enemyBattle.Remove(attacker);
                attacker.transform.SetParent(EnemyYard.transform, false);

            }
        }
        else if (attPower == defPower)
        {
            Debug.Log("[INFO] Equal power. Both creatures die.");
            if (hasAuthority)
            {
                enemyBattle.Remove(defender);
                defender.transform.SetParent(EnemyYard.transform, false);
                playerBattle.Remove(attacker);
                attacker.transform.SetParent(PlayerYard.transform, false);
            }
            else
            {
                enemyBattle.Remove(attacker);
                attacker.transform.SetParent(EnemyYard.transform, false);
                playerBattle.Remove(defender);
                defender.transform.SetParent(PlayerYard.transform, false);

            }
        }

    }

    public void AttackHead(GameObject attacker)
    {
        Debug.Log("Trying to attack head.");
        if (!attacker.GetComponent<Card>().canAttackPlayers)
        {
            Debug.Log("[INFO] Attacker could not attack player as it is not able.");
            attacker.transform.SetParent(PlayerBattleZone.transform, false);
            return;
        }
        if (attacker.GetComponent<Card>().isTapped)
        {
            Debug.Log("[INFO] Attacker could not attack creature as it is tapped.");
            attacker.transform.SetParent(PlayerBattleZone.transform, false);
            return;
        }
        attacker.GetComponent<Card>().isTapped = true;
        attacker.GetComponent<Outline>().enabled = true;
        CmdAttackHead(attacker);
    }

    [Command]
    void CmdAttackHead(GameObject attacker)
    {
        RpcAttackHead(attacker);
    }

    [ClientRpc]
    void RpcAttackHead(GameObject attacker)
    {
        if (hasAuthority)
        {
            foreach (GameObject card in enemyBattle)
            {
                if (card.GetComponent<Card>().isBlocker)
                {
                    AttackCard(attacker, card);
                    return;
                }
            }

        }
        
        int shields = attacker.GetComponent<Card>().shieldsBrokenOnAttack;
        if (hasAuthority)
        {
            for (int i = 0; i < shields; ++i)
            {
                GameObject auxCard = EnemyShields[curShieldIndex].transform.GetChild(0).gameObject;
                EnemyShields.RemoveAt(curShieldIndex);
                if (curShieldIndex == 0)
                {
                    Application.Quit();
                }
                curShieldIndex--;
                Debug.Log("One shield was lost.");
                auxCard.transform.SetParent(EnemyArea.transform, false);
            }
        } else
        {
            for (int i = 0; i < shields; ++i)
            {
                GameObject auxCard = PlayerShields[curShieldIndex].transform.GetChild(0).gameObject;
                PlayerShields.RemoveAt(curShieldIndex);
                if (curShieldIndex == 0)
                {
                    Application.Quit();
                }
                curShieldIndex--;
                auxCard.transform.SetParent(PlayerArea.transform, false);
                auxCard.GetComponent<Card>().Flip();
            }
        }
        


    }
}
