using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Titan : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Visuals")]
    [SerializeField] public string titanName;
    [SerializeField] string gender;
    [SerializeField] titanClass titanClass;
    [SerializeField] Sprite image;

    [Header("Level")]
    [SerializeField] public float lvl;
    [SerializeField] public float xp;

    [Header("HP")]
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;

    [Header("Attributes")]
    [SerializeField] float strength;
    [SerializeField] float dexterity;
    [SerializeField] float vitality;

    [Header("Skills")]
    [SerializeField] public SkillSO[] skills;

    [Header("BattleStats")]
    [SerializeField] float damage;
    [SerializeField] float armor;

    [Header("Equipment")]
    [SerializeField] Item[] equipment;
    [SerializeField] WeaponSO weapon;
    [SerializeField] ShieldSO shield;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHP);
            stream.SendNext(maxHP);
        }
        else
        {
            currentHP = (float)stream.ReceiveNext();
            maxHP = (float)stream.ReceiveNext();
        }
    }

    public void SetStats()
    {
        Debug.Log("Setting stats");
        SetBattleStats();
        Debug.Log("Finished setting stats");
    }

    private void SetBattleStats()
    {
        SetDamage();

        Debug.Log("Finished setting damage, starting UpdateBattleStats");

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("bam");
            Debug.Log(photonView);
            photonView.RPC("UpdateBattleStats", RpcTarget.AllBuffered, damage, armor, maxHP);
        }
        else
        {
            Debug.Log("ba,-bam");
            UpdateBattleStats(damage, armor, maxHP);
        }
        
    }

    private void SetDamage()
    {
        damage = strength;
    }

    [PunRPC]
    void UpdateBattleStats(float damage, float armor, float maxHP)
    {
        this.damage = damage;
        this.armor = armor;
        this.maxHP = maxHP;
    }

    public void TakeDamage(float damageReceived)
    {
        currentHP -= damageReceived;

        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("UpdateCurrentHP", RpcTarget.AllBuffered, currentHP);
        }
        else
        {
            UpdateCurrentHP(currentHP);
        }
        
    }

    [PunRPC]
    void UpdateCurrentHP(float currentHP)
    {
        this.currentHP = currentHP;
    }
}


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Photon.Pun;


//public class Titan : MonoBehaviourPunCallbacks, IPunObservable
//{
//    [Header("Visuals")]
//    [SerializeField] public string titanName;
//    [SerializeField] string gender;
//    [SerializeField] titanClass titanClass;
//    [SerializeField] Sprite image;


//    [Header("lvl")]
//    [SerializeField] public float lvl;
//    [SerializeField] public float xp;


//    [Header("HP")]
//    [SerializeField] public float maxHP;
//    [SerializeField] public float currentHP;

//    [Header("Attributes")]
//    [SerializeField] float strength_test;
//    [SerializeField] float dexterity_test;
//    [SerializeField] float vitality_test;
//    //[SerializeField] public Attribute str;
//    //[SerializeField] public Attribute dex;
//    //[SerializeField] public Attribute vit;
//    //[SerializeField] float strValue;
//    //[SerializeField] float dexValue;
//    //[SerializeField] float vitValue;


//    [Header("Skills")]
//    // [SerializeField] public SkillsSO[4] activeSkills;
//    [SerializeField] public SkillSO[] skills;


//    [Header("BattleStats")]
//    [SerializeField] float damage_test;
//    [SerializeField] float armor_test;
//    //[SerializeField] public Armor armor;
//    //[SerializeField] public float armorValue;
//    //[SerializeField] Damage damage;
//    //[SerializeField] float damageBasicValue;
//    //[SerializeField] float damageMinValue;
//    //[SerializeField] float damageMaxValue;


//    [Header("Equipment")]
//    //[SerializeField]
//    [SerializeField] Item[] equipment;
//    [SerializeField] WeaponSO weapon;
//    [SerializeField] ShieldSO shieled;
//    //[SerializeField] Equipment equipment;
//    WeaponLibrary weaponLibrary;
//    Titan titan;

//    public bool isMultiplayer = false;


//    private void Start()
//    {
//        if (photonView.IsMine)
//        {
//            isMultiplayer = true;
//        }
//    }

//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (stream.IsWriting)
//        {
//            // We own this player: send the others our data
//            stream.SendNext(currentHP);
//            stream.SendNext(maxHP);
//            // ... Send other attributes...
//        }
//        else
//        {
//            // Network player, receive data
//            currentHP = (float)stream.ReceiveNext();
//            maxHP = (float)stream.ReceiveNext();
//            // ... Receive other attributes...
//        }
//    }

//    //old and may be not usefull
//    ////does nothing at the moment
//    //public void UpdateStat(Stat stat, float modifier)
//    //{
//    //    Titan titan = GetComponent<Titan>();
//    //    stat.AddModifier(modifier);
//    //    stat.GetFinalStat();
//    //}

//    public void SetStats()
//    {
//        Debug.Log("Im setting stats");
//        titan = GetComponent<Titan>();
//        //GetItemAttributes(equipment);
//        //SetAttributes();
//        //GetPassiveBonuses(titan);
//        SetBattleStats();
//        Debug.Log("finished setting stats");

//    }

//    private void SetBattleStats()
//    {
//        SetDamage_test();
//        //SetArmor();
//        //SetHP();

//        if (isMultiplayer)
//        {
//            photonView.RPC("UpdateBattleStats", RpcTarget.AllBuffered, damage_test, armor_test, maxHP);
//        }
//    }

//    private void SetDamage_test()
//    {
//        damage_test = strength_test;
//    }

//    [PunRPC]
//    void UpdateBattleStats(float damage, float armor, float hp)
//    {
//        this.damage_test = damage;
//        this.armor_test = armor;
//        this.maxHP = hp;
//    }

//}