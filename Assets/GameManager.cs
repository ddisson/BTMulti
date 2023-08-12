using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
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

    public delegate void PlayerConnectedEvent();
    public static event PlayerConnectedEvent OnSinglePlayerConnected;
    public static event PlayerConnectedEvent OnBothPlayersConnected;

    private void Awake()
    {
        InitializeSingleton();
        LoadPrefabs();

        PhotonNetwork.ConnectUsingSettings();
    }

    private void InitializeSingleton()
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
    }

    private void LoadPrefabs()
    {
        player1Prefab = Resources.Load<GameObject>("Player");
        player2Prefab = Resources.Load<GameObject>("Enemy");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("GameManager: Connected to Master");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"GameManager: Failed to Join Random Room. Return Code: {returnCode}, Message: {message}");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public GameData ToGameData()
    {
        GameData gameData = new GameData
        {
            lvl = BattleSet.instance.playerTitan.lvl,
            xp = BattleSet.instance.playerTitan.xp
        };

        return gameData;
    }

    public void SaveGame()
    {
        GameData gameData = ToGameData();
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
            LoadFromGameData(gameData);
        }
        else
        {
            Debug.Log("No save file found");
        }
    }

    private void LoadFromGameData(GameData gameData)
    {
        BattleSet.instance.playerTitan.lvl = gameData.lvl;
        BattleSet.instance.playerTitan.xp = gameData.xp;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("GameManager: Joined a Room");
        if (PhotonNetwork.IsConnected)
        {
            SetupPlayers();

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                OnSinglePlayerConnected?.Invoke();
                Debug.Log("[GameManager] OnSinglePlayerConnected event invoked.");

            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                Debug.Log("2 PLAYERS HERE, BRO");
                OnBothPlayersConnected?.Invoke();
                Debug.Log("[GameManager] OnBothPlayersConnected event invoked.");

            }
            else
            {
                Debug.Log("I dont see other player");
            }
        }
    }

    private void SetupPlayers()
    {
        if (clientID == 1)
        {
            SetupPlayer1();
            Debug.Log("player ID=1 is set");
        }
        else if (clientID == 2)
        {
            SetupPlayer2();
            Debug.Log("player ID=2 is set");
        }
    }

    private void SetupPlayer1()
    {
        Debug.Log("Setting player 1 on client with ID: " + clientID);
        Titan playerTitan = BattleSet.instance.SetPlayer1(player1Prefab, playerPosition);
        NewTargeting newTargeting = NewTargeting.instance;
        newTargeting.playerTitan = playerTitan;
        newTargeting.Initialize(player1AttackButtons, player1DefendButtons);
    }

    private void SetupPlayer2()
    {
        Debug.Log("Setting player 2 on client with ID: " + clientID);
        Titan enemyTitan = BattleSet.instance.SetPlayer2(player2Prefab, enemyPosition);
        NewTargeting newTargeting = NewTargeting.instance;
        newTargeting.playerTitan = enemyTitan;
        newTargeting.Initialize(player2AttackButtons, player2DefendButtons);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"GameManager: Player {newPlayer.NickName} entered the room");

        UpdateHuds();
        Debug.Log("Hud Updated on Playerejoinedroom");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("2 PLAYERS HERE, BRO");
            OnBothPlayersConnected?.Invoke();
            Debug.Log("[GameManager] OnBothPlayersConnected event invoked.");

        }
        else
        {
            Debug.Log("I dont see other player");
        }
    }

    private void UpdateHuds()
    {
        if (clientID == 1)
        {
            if (BattleSet.instance.playerTitan != null)
            {
                BattleSet.instance.playerHud.UpdateHud(BattleSet.instance.playerTitan);
                Debug.Log("UpdatedHud for plaerTitan. Im ID=1");
            }

            if (BattleSet.instance.enemyTitan != null)
            {
                BattleSet.instance.enemyHud.UpdateHud(BattleSet.instance.enemyTitan);
                Debug.Log("UpdatedHud for enemyTitan. Im ID=1");
            }

        }
        else if (clientID == 2)
        {
            if (BattleSet.instance.playerTitan != null)
            {
                BattleSet.instance.playerHud.UpdateHud(BattleSet.instance.playerTitan);
                Debug.Log("UpdatedHud for plaerTitan. Im ID=2");
            }

            if (BattleSet.instance.enemyTitan != null)
            {
                BattleSet.instance.enemyHud.UpdateHud(BattleSet.instance.enemyTitan);
                Debug.Log("UpdatedHud for enemyTitan. Im ID=2");
            }
        }
    }
}


//public override void OnJoinedRoom()
//{
//    Debug.Log("Joined a room");

//    if (PhotonNetwork.IsConnected)
//    {
//        if (clientID == 1)
//        {
//            //Debug.Log("Setting player 1 on client with ID: " + clientID);
//            //BattleSet.instance.SetPlayer1(player1Prefab, playerPosition);
//            //// Access the NewTargeting singleton instance
//            //NewTargeting newTargeting = NewTargeting.instance;
//            //newTargeting.playerTitan = BattleSet.instance.playerTitan;
//            //newTargeting.Initialize(player1AttackButtons, player1DefendButtons);

//            Debug.Log("Setting player 1 on client with ID: " + clientID);
//            Titan playerTitan = BattleSet.instance.SetPlayer1(player1Prefab, playerPosition);
//            Debug.Log(playerTitan + "HI I 'm here man before targeting!1");
//            // Access the NewTargeting singleton instance
//            NewTargeting newTargeting = NewTargeting.instance;
//            newTargeting.playerTitan = playerTitan;
//            Debug.Log(playerTitan + "HI I 'm here man! after1");
//            newTargeting.Initialize(player1AttackButtons, player1DefendButtons);

//        }
//        else if (clientID == 2)
//        {
//            //Debug.Log("Setting player 2 on client with ID: " + clientID);
//            //BattleSet.instance.SetPlayer2(player2Prefab, enemyPosition);
//            //NewTargeting newTargeting = NewTargeting.instance;
//            //newTargeting.playerTitan = BattleSet.instance.enemyTitan;
//            //newTargeting.Initialize(player2AttackButtons, player2DefendButtons);

//            Debug.Log("Setting player 2 on client with ID: " + clientID);
//            Titan enemyTitan = BattleSet.instance.SetPlayer2(player2Prefab, enemyPosition);
//            Debug.Log(enemyTitan + "HI I 'm here man before targeting!");
//            NewTargeting newTargeting = NewTargeting.instance;
//            newTargeting.playerTitan = enemyTitan;
//            Debug.Log(enemyTitan + "HI I 'm here man! after");
//            newTargeting.Initialize(player2AttackButtons, player2DefendButtons);

//        }

//        // Raise an event to let other players know a player has joined - NOT woring, may check and delete
//        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
//        PhotonNetwork.RaiseEvent(BattleSet.EventCodes.PlayerJoined, null, raiseEventOptions, SendOptions.SendReliable);

//        Debug.Log("Both players joined");
//    }
//    else
//    {
//        Debug.Log("Waiting for the other player to join...");
//    }


//    // Add any other required functions
//}

//public override void OnEnable()
//{
//    base.OnEnable();
//    PhotonNetwork.AddCallbackTarget(this);
//}

//public override void OnDisable()
//{
//    base.OnDisable();
//    PhotonNetwork.RemoveCallbackTarget(this);
//}

//BELOW IS EVENT TO CHECH UD OF CONNECTEDE PLAYERES
//public void OnEvent(EventData photonEvent)
//{
//    StartCoroutine(OnEventCoroutine(photonEvent));
//}

//private IEnumerator OnEventCoroutine(EventData photonEvent)
//{
//    byte eventCode = photonEvent.Code;

//    if (eventCode == EventCodes.PlayerJoined)
//    {
//        // Wait until the playerTitan object is not null
//        yield return new WaitUntil(() => playerTitan != null);

//        if (GameManager.instance.clientID == 1)
//        {
//            playerHud.UpdateHud(playerTitan);
//        }
//        else
//        {
//            enemyHud.UpdateHud(enemyTitan);
//        }
//    }
//}

//updating hud fo both players to be visible
//public override void OnPlayerEnteredRoom(Player newPlayer)
//{
//    if (clientID == 1)
//    {
//        Debug.Log("I'm on overiding cliendid 1");
//        BattleSet.instance.playerHud.UpdateHud(BattleSet.instance.playerTitan);
//        BattleSet.instance.enemyHud.UpdateHud(BattleSet.instance.enemyTitan);

//    }
//    else if (clientID == 2)
//    {
//        Debug.Log("I'm on overiding cliendid 2");
//        BattleSet.instance.enemyHud.UpdateHud(BattleSet.instance.enemyTitan);
//        BattleSet.instance.playerHud.UpdateHud(BattleSet.instance.playerTitan);
//    }

//    if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
//    {
//        Debug.Log("2 PLAYERS HERE, BRO");
//    }
//}