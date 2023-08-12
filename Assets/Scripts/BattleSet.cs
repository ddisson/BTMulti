using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon; // For EventData

public class BattleSet : MonoBehaviourPunCallbacks
{

    private const byte player2JoinedEventCode = 0;
    private const byte requestHudUpdateEventCode = 1;

    public static BattleSet instance = null;
 

    public Transform enemyPosition;
    public Transform playerPosition;

    public Titan playerTitan;
    public Titan enemyTitan;

    GameObject player1Prefab;
    GameObject player2Prefab;

    public BattleHud playerHud;
    public BattleHud enemyHud;

    TextMeshProUGUI battleconsole;

    [SerializeField] Button[] skillButtons;

    public static class EventCodes
    {
        public const byte PlayerJoined = 1;
    }


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
        photonView = GetComponent<PhotonView>();
    }

    
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager is not initialized");
            return;
        }

        GameObject player1Prefab = GameManager.instance.player1Prefab;
        GameObject player2Prefab = GameManager.instance.player2Prefab;

    }

    public void SetBattle(Titan playerTitan, Titan enemyTitan)
    {
        //player1Titan = playerTitan;
        //player2Titan = enemyTitan;

        Debug.Log("Setting up players");

        //SetPlayer();
    }

    public Titan SetPlayer1(GameObject playerPrefab, Transform titanPosition)
    {
        Debug.Log("Titan 1 is set, preparing to set a HUD");
        GameObject playerGO;
        playerGO = PhotonNetwork.Instantiate(playerPrefab.name, titanPosition.position, titanPosition.rotation);
        playerTitan = playerGO.GetComponent<Titan>();
        // Set the HUDs
        playerHud.SetHud(playerTitan);
        SetSkills(playerTitan);
        playerTitan.SetStats();
        Debug.Log("exiting the SetPlayer1 script");
        return playerTitan;
    }

    public Titan SetPlayer2(GameObject playerPrefab, Transform titanPosition)
    {
        Debug.Log("Titan 2 is set, preparing to set a HUD");
        GameObject enemyGO;
        enemyGO = PhotonNetwork.Instantiate(playerPrefab.name, titanPosition.position, titanPosition.rotation);
        enemyTitan = enemyGO.GetComponent<Titan>();
        Debug.Log("pre Hud titan 2");
        enemyHud.SetHud(enemyTitan);
        Debug.Log("hud set, setting skills 2");
        SetSkills(enemyTitan);
        Debug.Log("skills set, setting stats 2");
        enemyTitan.SetStats();
        //playerHud.UpdateHud(playerTitan);
        Debug.Log("exiting the SetPlayer2 script");
        return enemyTitan;

    }



    private void SetSkills(Titan titan)
    {
        Debug.Log("begin to set skills");
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (titan.skills[i] != null)
            {
                Image skillImg = skillButtons[i].GetComponent<Image>();
                skillImg.sprite = titan.skills[i].skillIcon;
            }
        }
        Debug.Log("finish to set skills");
    }

    //public override void OnEnable()
    //{
    //    PhotonNetwork.AddCallbackTarget(this);
    //    base.OnEnable();
    //}

    //public override void OnDisable()
    //{
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //    base.OnDisable();
    //}

    //public void OnEvent(EventData photonEvent)
    //{
    //    if (photonEvent.Code == battleConsole.instance.BattleConsoleEventCode)
    //    {
    //        object[] data = (object[])photonEvent.CustomData;
    //        string line = (string)data[0];
    //        consoleText.text += "\n" + line;
    //    }
    //}



}

