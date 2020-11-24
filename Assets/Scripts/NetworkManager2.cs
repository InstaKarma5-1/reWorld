﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Linq;

public class NetworkManager2 : NetworkManager
{
    [Header("Game Objects")]
    public GameObject GargoyleNPC;
    public GameObject SignBoard;
    public EnemySpawner es;

    public GameObject[] playerList;
    public uint[] playerId;
    public char[] playerClass;

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("Saving player classes before changing out of PreGameLobby");
            playerList = GameObject.FindGameObjectsWithTag("Player");
            playerId = new uint[playerList.Length];
            playerClass = new char[playerList.Length];
            for (int i = 0; i < playerList.Length; i++)
            {
                Player player = playerList[i].GetComponent<Player>();
                playerClass[i] = player.playerClass;
                playerId[i] = player.netIdentity.netId;
            }

        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("Server scene changed to PreGameLobby");
            SpawnStuffs();
        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Debug.Log("Server scene changed to Map 2");
            es = FindObjectOfType<EnemySpawner>();
            SpawnMobs();


        }
    }

    /*    public override void OnServerAddPlayer(NetworkConnection conn)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            NetworkServer.AddPlayerForConnection(conn, player);

            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                LobbyManager lobby = FindObjectOfType<LobbyManager>();
                lobby.player.Append<GameObject>(player.GetComponent<Player>);
            }
        }*/

    public void SpawnStuffs()
    {
        //Spawn gargoyle
        GameObject clone = Instantiate(GargoyleNPC);
        NetworkServer.Spawn(clone);

        //Spawn signboard
        GameObject signboard = Instantiate(SignBoard);
        NetworkServer.Spawn(signboard);
    }

    public void SpawnMobs()
    {
/*        es.RpcInitialSpawn(es.Enemies[0], 8);
        es.RpcInitialSpawn(es.Enemies[1], 4);
        es.RpcInitialSpawn(es.Enemies[2], 1);
        es.RpcInitialSpawn(es.Enemies[3], 8);
        es.RpcInitialSpawn(es.Enemies[4], 4);*/
    }

    public void checkPlayerClass(uint id)
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Player player = NetworkIdentity.spawned[id].gameObject.GetComponent<Player>();
            int index = 0;
            for(int i = 0; i < playerId.Length; i++)
            {
                if(playerId[i] == id)
                {
                    index = i;
                }
            }
            player.playerClass = playerClass[index];
            player.activateClassScripts();
        }
    }
}
