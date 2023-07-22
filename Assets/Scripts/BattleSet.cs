using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon; // For EventData

public class BattleSet : MonoBehaviourPunCallbacks, IOnEventCallback
{

    private const byte player2JoinedEventCode = 0;
    private const byte requestHudUpdateEventCode = 1;

    public static BattleSet instance = null;
    public new PhotonView photonView;

    public Transform enemyPosition;
    public Transform playerPosition;

    public Titan playerTitan;
    public Titan enemyTitan;

    GameObject player1Prefab;
    GameObject player2Prefab;

    public BattleHud playerHud;
    public BattleHud enemyHud;

    [SerializeField] Button[] skillButtons;

    public static class EventCodes
    {
        public const byte PlayerJoined = 0;
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

    public void SetPlayer1(GameObject playerPrefab, Transform titanPosition)
    {
        Debug.Log("Titan is set, preparing to set a HUD");
        GameObject playerGO;
        playerGO = PhotonNetwork.Instantiate(playerPrefab.name, titanPosition.position, titanPosition.rotation);
        playerTitan = playerGO.GetComponent<Titan>();
        // Set the HUDs
        playerHud.SetHud(playerTitan);
        SetSkills(playerTitan);
        playerTitan.SetStats();
    }

    public void SetPlayer2(GameObject playerPrefab, Transform titanPosition)
    {
        Debug.Log("Titan is set, preparing to set a HUD");
        GameObject enemyGO;
        enemyGO = PhotonNetwork.Instantiate(playerPrefab.name, titanPosition.position, titanPosition.rotation);
        enemyTitan = enemyGO.GetComponent<Titan>();
        // Set the HUDs
        enemyHud.SetHud(enemyTitan);
        SetSkills(enemyTitan);
        enemyTitan.SetStats();
        playerHud.UpdateHud(playerTitan);
    }



    private void SetSkills(Titan titan)
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (titan.skills[i] != null)
            {
                Image skillImg = skillButtons[i].GetComponent<Image>();
                skillImg.sprite = titan.skills[i].skillIcon;
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        StartCoroutine(OnEventCoroutine(photonEvent));
    }

    private IEnumerator OnEventCoroutine(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == EventCodes.PlayerJoined)
        {
            // Wait until the playerTitan object is not null
            yield return new WaitUntil(() => playerTitan != null);

            if (GameManager.instance.clientID == 1)
            {
                playerHud.UpdateHud(playerTitan);
            }
            else
            {
                enemyHud.UpdateHud(enemyTitan);
            }
        }
    }

}

