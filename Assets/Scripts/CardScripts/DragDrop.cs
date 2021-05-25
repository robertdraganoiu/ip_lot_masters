using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
//using System.Diagnostics;

public class DragDrop : NetworkBehaviour
{
    public GameManager GameManager;
    public GameObject Canvas;
    public PlayerManager PlayerManager;

    private bool isDragging = false;
    private bool isOverMyBattleZone = false;
    private bool isOverEnemyBattleZone = false;
    private bool isOverManaZone = false;
    private bool isDraggable = true;
    private bool isOverAvatar = false;
    private GameObject myBattleZone;
    private GameObject enemyBattleZone;
    private GameObject defender;
    private GameObject manaZone;
    private GameObject startParent;
    private Vector2 startPosition;

    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Canvas = GameObject.Find("Main Canvas");
        NetworkIdentity netWorkIdentity = NetworkClient.connection.identity;
        PlayerManager = netWorkIdentity.GetComponent<PlayerManager>();

        if (!hasAuthority)
        {
            isDraggable = false;
        }
    }
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colission Object: " + collision.gameObject);
        //TODO ENTER BATTLEGROUND (Now it checks collision with shields)
        if (startParent == PlayerManager.PlayerArea)
        {
            //Modify 
            if (collision.gameObject == PlayerManager.PlayerBattleZone)
            {
                isOverMyBattleZone = true;
                myBattleZone = collision.gameObject;

            }

            if (collision.gameObject == PlayerManager.PlayerManaZone)
            {
                isOverManaZone = true;
                manaZone = collision.gameObject;
            }
        }
        //TODO ATTACK
        if (startParent == PlayerManager.PlayerBattleZone)
        {
            Debug.Log("Showdown 11");
            
            if (collision.gameObject.transform.parent.gameObject == PlayerManager.EnemyBattleZone)
            {
                isOverEnemyBattleZone = true;
                enemyBattleZone = collision.gameObject;
                Debug.Log("Showdown 112");
            }

            
            //Debug.Log("Colission Parent: " + collision.gameObject.transform.parent.gameObject);
            Debug.Log("Should be Parent: " + PlayerManager.EnemyAvatar);
            if (collision.gameObject == PlayerManager.EnemyAvatar)
            {
                Debug.Log("Showdown 113");
                isOverAvatar = true;
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverEnemyBattleZone = false;
        isOverAvatar = false;
        isOverMyBattleZone = false;
        isOverManaZone = false;
        myBattleZone = null;
        manaZone = null;
    }

    public void StartDrag()
    {
        Debug.Log("ii falsa? " + isDraggable);
        if (!isDraggable) return;
        startParent = transform.parent.gameObject;
        Debug.Log("Start parent: " + startParent);
        startPosition = transform.position;
        isDragging = true;
    }

    public void EndDrag()
    {   
        if (!isDraggable) return;
        isDragging = false;

        if (isOverMyBattleZone && PlayerManager.id == GameManager.currentPlayerTurn)
        {
            Debug.Log("Differce:   " + PlayerManager.currentMana);
            if (PlayerManager.currentMana - gameObject.GetComponent<Card>().mana >= 0)
            {
                Debug.Log("Entered...");
                PlayerManager.currentMana -= gameObject.GetComponent<Card>().mana;
                transform.SetParent(myBattleZone.transform, false);
                PlayerManager.PlayCard(gameObject, "MyBattleZone");
            } 
            else
            {
                transform.position = startPosition;
                transform.SetParent(startParent.transform, false);
            }
        }
        else if (isOverManaZone && PlayerManager.id == GameManager.currentPlayerTurn && !PlayerManager.hasIncreasedMana)
        {
            transform.SetParent(manaZone.transform, false);
            isDraggable = false;
            
            PlayerManager.PlayCard(gameObject, "Mana");
            
            PlayerManager.hasIncreasedMana = true;
        }
        else if (isOverEnemyBattleZone && PlayerManager.id == GameManager.currentPlayerTurn)
        {   
            PlayerManager.AttackCard(gameObject, enemyBattleZone);
        }
        else if (isOverAvatar && PlayerManager.id == GameManager.currentPlayerTurn)
        {
            Debug.Log("Collision with Enemy Avatar working.");
            PlayerManager.AttackHead(gameObject);
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }
}
