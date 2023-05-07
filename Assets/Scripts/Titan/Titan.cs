using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Titan : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] public string titanName;
    [SerializeField] string gender;
    [SerializeField] titanClass titanClass;
    [SerializeField] Sprite image;


    [Header("lvl")]
    [SerializeField] public float lvl;
    [SerializeField] public float xp;


    [Header("HP")]
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;

    [Header("Attributes")]
    [SerializeField] public Attribute str;
    [SerializeField] public Attribute dex;
    [SerializeField] public Attribute vit;
    [SerializeField] public float strValue;
    [SerializeField] float dexValue;
    [SerializeField] float vitValue;


    [Header("Skills")]
    // [SerializeField] public SkillsSO[4] activeSkills;
    [SerializeField] public SkillSO[] skills;
    

    [Header("BattleStats")]
    [SerializeField] public Armor armor;
    [SerializeField] public float armorValue;
    [SerializeField] Damage damage;
    [SerializeField] float damageBasicValue;
    [SerializeField] float damageMinValue;
    [SerializeField] float damageMaxValue;
    //[SerializeField] Stat blockChance;
    //[SerializeField] Stat critChance;
    //[SerializeField] Stat critMultiplier;

    [Header("Equipment")]
    //[SerializeField]
    [SerializeField] Item[] equipment;
    [SerializeField] WeaponSO weapon;
    [SerializeField] ShieldSO shieled;
    //[SerializeField] Equipment equipment;
    WeaponLibrary weaponLibrary;
    Titan titan;



    private void Start()
    {
        //refference to himself
        
        Debug.Log("Im at start");
        //SetStats();
    }

    //does nothing at the moment
    public void UpdateStat(Stat stat, float modifier)
    {
        Titan titan = GetComponent<Titan>();
        stat.AddModifier(modifier);
        stat.GetFinalStat();
    }

    public void SetStats()
    {
        Debug.Log("Im setting stats");
        titan = GetComponent<Titan>();
        GetItemAttributes(equipment);
        SetAttributes();
        GetPassiveBonuses(titan);
        SetBattleStats();
        Debug.Log("finished setting stats");

    }

    private void SetBattleStats()
    {
        SetDamage();
        SetArmor();
        SetHP();
    }

    private void SetHP()
    {
        AddVitToHP();
        maxHP = vit.GetFinalStat(); //change to formula

   //     AddBonusItemHPH();
    }

    private void AddVitToHP()
    {
        vit.AddModifier(vitValue); // change formula
    }

    private void SetArmor()
    {
        AddDexToDamage(); //from stats
        AddBonusItemArmor(); // from equipment
        armorValue = armor.GetFinalStat();

    }

    private void AddBonusItemArmor()
    {
        foreach (var item in equipment)
        {
            if (item != null)
            {
                armor.AddModifier(item.armorModifier);
            }

        }
    }

    private void SetDamage()
    {
        AddStrToDamage(); //from stats
        AddBonusItemDamage(); // from equipment
        SetDamageBasicValue(); //fin basic: stats + modifiers
        SetMinDamage(damageBasicValue); //basic damage + min weapon range
        SetMaxDamage(damageBasicValue); //basic damage + max weapon range
        Debug.Log("finished damage");

    }

    private void SetMaxDamage(float damageBasicValue)
    {
        float weaponMaxDamage = 0;

        foreach (var item in equipment)
        {
            if (item.itemType == ItemType.Weapon)
            {
                var weapon = (WeaponSO)item;
                weaponMaxDamage += weapon.damageMax;

            }
        }

        damageMaxValue = damageBasicValue + weaponMaxDamage;
    }

    private void SetMinDamage(float damageBasicValue)
    {
        float weaponMinDamage = 0;

        foreach (var item in equipment)
        {
            if (item.itemType == ItemType.Weapon)
            {
                var weapon = (WeaponSO)item;
                weaponMinDamage += weapon.damageMin;

            }
        }

        damageMinValue = damageBasicValue + weaponMinDamage;
    }

    private void SetDamageBasicValue()
    {
        damageBasicValue = damage.GetFinalStat();
    }

    private void AddBonusItemDamage()
    {
        foreach (var item in equipment)
        {
            if (item != null)
            {
                damage.AddModifier(item.damageModifier);
            }

        }
    }

    private void AddStrToDamage()
    {
        damage.AddModifier(strValue);
    }

    public void GetItemAttributes(Item[] equipment)
    {
        foreach (var item in equipment)
        {
            if (item != null)
            {
                str.AddModifier(item.strModifier);
                dex.AddModifier(item.dexModifier);
                vit.AddModifier(item.vitModifier);
            }

        }
        Debug.Log("I added Atrs");
    }

    private void SetAttributes()
    {
        Debug.Log("Im setting Atrs");
        strValue = str.GetFinalStat();
        dexValue = dex.GetFinalStat();
        vitValue = vit.GetFinalStat();
    }

    private void GetPassiveBonuses(Titan titan)
    {
        foreach (var skill in skills)
        {
            if (skill.skillType == SkillType.Passive)
            {
                skill.EffectPassiveTargeted(titan);
                skill.EffectPassive();

            }
        }
    }

    private void AddDexToDamage()
    {
        armor.AddModifier(dexValue); //add normal formula
    }





}







