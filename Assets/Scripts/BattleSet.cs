using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BattleSet : MonoBehaviourPunCallbacks
{
    public new PhotonView photonView;

    [SerializeField] public GameObject playerPrefab;

    public GameObject enemyPrefab;

    public Transform enemyPosition;
    public Transform playerPosition;

    public Titan playerTitan;
    public Titan enemyTitan;
    public string[] titanPrefabNames;
    [SerializeField] private string playerID;


    public BattleHud playerHud;
    public BattleHud enemyHud;

    [SerializeField] Button[] skillButtons;

    void Start()
    {
        Debug.Log("Player joined");
        Debug.Log("Setting up the battle for you");
        PhotonNetwork.ConnectUsingSettings();
        SetBattle();
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
        SetBattle();
    }



    private void SetBattle()
    {
        Debug.Log("I want to create player");
        SetPlayer();
    }

    private void SetPlayer()
    {
        SetTitan();
        Debug.Log("Titan is set, preeparing to seet a hud");
        
        
        Debug.Log("Script finished successfully");
    }

    //public void SetHud(Titan titan)
    //{
    //    Debug.Log("setting hud");
    //    nameText.text = titan.titanName;
    //    hpSlider.maxValue = titan.maxHP;
    //    hpSlider.value = titan.currentHP;
    //    hpText.text = titan.currentHP.ToString();
    //}

    private void SetTitan()
    {
        GameObject playerPrefab;

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Loading multi-player mode titan");
            if (playerID == "0")
            {
                Debug.Log("loading knight");
                playerPrefab = Resources.Load<GameObject>(titanPrefabNames[0]);
                GameObject playerGO = PhotonNetwork.Instantiate(playerPrefab.name, playerPosition.position, playerPosition.rotation);
                playerTitan = playerGO.GetComponent<Titan>();
                playerHud.SetHud(playerTitan);
                SetSkills(playerTitan);
            }
            else if (playerID == "1")
            {
                Debug.Log("loading skeleton");
                playerPrefab = Resources.Load<GameObject>(titanPrefabNames[1]);
                GameObject playerGO = PhotonNetwork.Instantiate(playerPrefab.name, enemyPosition.position, enemyPosition.rotation);
                enemyTitan = playerGO.GetComponent<Titan>();
                enemyHud.SetHud(enemyTitan);
                SetSkills(enemyTitan);
            }
            else
            {
                // handle error here if no valid playerID is set
                Debug.Log("currently no playerID valid");
                return;
            }

            
            //playerGO.transform.localScale = titanSize; // Setting the scale

            
            playerTitan.SetStats();
        }
        else
        {
            Debug.Log("Loading single-player mode titan");
            playerPrefab = Resources.Load<GameObject>(titanPrefabNames[0]); // or another specific prefab for single-player mode
            GameObject playerGO = Instantiate(playerPrefab, playerPosition.position, playerPosition.rotation);
            playerTitan = playerGO.GetComponent<Titan>();
            playerTitan.SetStats();
        }


    }


    private void SetSkills(Titan titan)
    {
        Image skill1IMG;

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (titan.skills[i] != null)
            {
                skill1IMG = skillButtons[i].GetComponent<Image>();
                skill1IMG.sprite = titan.skills[i].skillIcon;
            }
        }
    }
}
