using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Head,
    Torso,
    Weapon,
    Shield,
    Legs,
    Ring

}

[CreateAssetMenu(fileName = "New item", menuName = "Item")]
public class Item : ScriptableObject

{
    [SerializeField] public int itemID;
    [SerializeField] string itemName;
    [SerializeField] public ItemType itemType;

    [Header("Atr Modifiers")]
    [SerializeField] public float strModifier;
    [SerializeField] public float dexModifier;
    [SerializeField] public float vitModifier;

    [Header("BattlStats Modifiers")]
    [SerializeField] public float damageModifier;
    [SerializeField] public float armorModifier;

}
