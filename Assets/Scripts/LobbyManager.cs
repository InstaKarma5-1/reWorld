﻿using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public Player player;
    public Button warrior, mage;
    [Header("UI")]
    public GameObject clientUI;
    public GameObject hostUI;
    public Text hostReadyNumberText, hostPlayerReadyText, classNameText;
    public Button hostStartGameButton, clientReadyButton;
    public SceneAsset map2;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        warrior.onClick.AddListener(() => { player.CmdRequestWarriorClass();});
        mage.onClick.AddListener(() => { player.CmdRequestMageClass();});
        clientReadyButton.onClick.AddListener(() => { player.CmdReadyUp(); });
        hostStartGameButton.onClick.AddListener(rpcStartGame);

        if (isClientOnly)
        {
            clientUI.SetActive(true);
            hostUI.SetActive(false);
        }
        else
        {
            clientUI.SetActive(false);
            hostUI.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        checkReady();

    }

    private void checkReady()
    {
        Player[] allPlayers = FindObjectsOfType<Player>();
        int numOfPlayers = allPlayers.Length;
        int playersReady = 0;

        foreach (Player player in allPlayers)
        {
            //Check player is ready
            if (player.isReady)
            {
                playersReady++;
            }

            //Check plyaer class and update UI
            if(player.playerClass == 'm')
            {
                classNameText.text = "Current class:Mage";
            }
            else if (player.playerClass == 'w')
            {
                classNameText.text = "Current class:Warrior";
            }
        }

        //Update UI
        //Checking start condition
        if (playersReady == numOfPlayers)
        {
            hostStartGameButton.interactable = true;
            hostStartGameButton.GetComponentInChildren<Text>().text = "<color=green>Start</color>";
            hostPlayerReadyText.text = "<color=green>Players Ready:</color>";
            hostReadyNumberText.text = "<color=green>" + playersReady + " / " + numOfPlayers + "</color>";
        }
        else{
            hostReadyNumberText.text = "<color=yellow>" + playersReady + " / " + numOfPlayers + "</color>";
            hostPlayerReadyText.text = "<color=yellow>Players Ready:</color>";
        }


    }

    [ClientRpc]
    public void rpcReady(uint id)
    {
        Player player = NetworkIdentity.spawned[id].gameObject.GetComponent<Player>();
        if (player.isReady == true)
        {
            player.isReady = false;
            clientReadyButton.GetComponentInChildren<Text>().text = "<color=red>Not ready</color>";
        }
        else
        {
            player.isReady = true; 
            clientReadyButton.GetComponentInChildren<Text>().text = "<color=green>Ready</color>";
        }
    }

    [ClientRpc]
    public void rpcStartGame()
    {
        NetworkManager2 nm2 = FindObjectOfType<NetworkManager2>();
        nm2.ServerChangeScene(map2.name);
    }

}
