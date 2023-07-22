using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static GameManager instance = null;

    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public Transform enemyPosition;
    public Transform playerPosition;

    [SerializeField] public int clientID;

    public Targeting targeting;

    public Button[] player1AttackButtons;
    public Button[] player1DefendButtons;
    public Button[] player2AttackButtons;
    public Button[] player2DefendButtons;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        player1Prefab = Resources.Load<GameObject>("Player");
        player2Prefab = Resources.Load<GameObject>("Enemy");

        // Connect to Photon servers
        PhotonNetwork.ConnectUsingSettings();


    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");

        if (PhotonNetwork.IsConnected)
        {
            if (clientID == 1)
            {
                //Debug.Log("Setting player 1 on client with ID: " + clientID);
                //BattleSet.instance.SetPlayer1(player1Prefab, playerPosition);
                //// Access the NewTargeting singleton instance
                //NewTargeting newTargeting = NewTargeting.instance;
                //newTargeting.playerTitan = BattleSet.instance.playerTitan;
                //newTargeting.Initialize(player1AttackButtons, player1DefendButtons);

                Debug.Log("Setting player 1 on client with ID: " + clientID);
                Titan playerTitan = BattleSet.instance.SetPlayer1(player1Prefab, playerPosition);
                Debug.Log(playerTitan + "HI I 'm here man before targeting!1");
                // Access the NewTargeting singleton instance
                NewTargeting newTargeting = NewTargeting.instance;
                newTargeting.playerTitan = playerTitan;
                Debug.Log(playerTitan + "HI I 'm here man! after1");
                newTargeting.Initialize(player1AttackButtons, player1DefendButtons);

            }
            else if (clientID == 2)
            {
                //Debug.Log("Setting player 2 on client with ID: " + clientID);
                //BattleSet.instance.SetPlayer2(player2Prefab, enemyPosition);
                //NewTargeting newTargeting = NewTargeting.instance;
                //newTargeting.playerTitan = BattleSet.instance.enemyTitan;
                //newTargeting.Initialize(player2AttackButtons, player2DefendButtons);

                Debug.Log("Setting player 2 on client with ID: " + clientID);
                Titan enemyTitan = BattleSet.instance.SetPlayer2(player2Prefab, enemyPosition);
                Debug.Log(enemyTitan + "HI I 'm here man before targeting!");
                NewTargeting newTargeting = NewTargeting.instance;
                newTargeting.playerTitan = enemyTitan;
                Debug.Log(enemyTitan + "HI I 'm here man! after");
                newTargeting.Initialize(player2AttackButtons, player2DefendButtons);

            }

            // Raise an event to let other players know a player has joined - NOT woring, may check and delete
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(BattleSet.EventCodes.PlayerJoined, null, raiseEventOptions, SendOptions.SendReliable);

            Debug.Log("Both players joined");
        }
        else
        {
            Debug.Log("Waiting for the other player to join...");
        }


        // Add any other required functions
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    //updating hud fo both players to be visible
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (clientID == 1)
        {
            Debug.Log("I'm on overiding cliendid 1");
            BattleSet.instance.playerHud.UpdateHud(BattleSet.instance.playerTitan);
            BattleSet.instance.enemyHud.UpdateHud(BattleSet.instance.enemyTitan);

        }
        else if (clientID == 2)
        {
            Debug.Log("I'm on overiding cliendid 2");
            BattleSet.instance.enemyHud.UpdateHud(BattleSet.instance.enemyTitan);
            BattleSet.instance.playerHud.UpdateHud(BattleSet.instance.playerTitan);
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("2 PLAYERS HERE, BRO");
        }
    }

    public GameData ToGameData()
    {
        GameData gameData = new GameData();
        gameData.lvl = BattleSet.instance.playerTitan.lvl;
        gameData.xp = BattleSet.instance.playerTitan.xp;
        return gameData;
    }

    public void LoadFromGameData(GameData gameData)
    {
        BattleSet.instance.playerTitan.lvl = gameData.lvl;
        BattleSet.instance.playerTitan.xp = gameData.xp;
    }

    public void SaveGame()
    {
        GameData gameData = this.ToGameData();
        string json = JsonUtility.ToJson(gameData);

        string path = Application.persistentDataPath + "/save.json";
        File.WriteAllText(path, json);
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            this.LoadFromGameData(gameData);
        }
        else
        {
            Debug.Log("No save file found");
        }
    }


}