using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIManager : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public GameManager GameManager;

    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        UpdateButtonText("Ready?");
    }

    public void UpdateButtonText(string text)
    {
        GameObject button = GameObject.Find("Button");
        button.GetComponentInChildren<Text>().text = text;
    }
}
