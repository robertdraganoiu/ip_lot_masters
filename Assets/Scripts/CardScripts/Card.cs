using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Card : NetworkBehaviour
{
    public PlayerManager PlayerManager;

    public string cardName;
    public int power;
    public int mana;
    public Sprite cardFront;
    public Sprite cardBack;
    public bool isSpell = false;
    public bool isTapped = false;
    public bool isBlocker = false;
    public bool canAttackCreatures = true;
    public bool canAttackPlayers = true;
    public int shieldsBrokenOnAttack = 1;
    public int attackBonus = 0;
    public void onAttacking()
    {
        power += attackBonus;
    }

    public void onFinishingAttack()
    {
        power -= attackBonus;
    }

    public void Flip()
    {
        if (gameObject.GetComponent<Image>().sprite.Equals(cardFront))
        {
            Debug.Log("Flipped to back.");
            gameObject.GetComponent<Image>().sprite = cardBack;
        }
        else
        {
            Debug.Log("Flipped to front.");
            gameObject.GetComponent<Image>().sprite = cardFront;
        }
    }
}
