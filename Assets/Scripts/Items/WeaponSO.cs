using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New weapon", menuName = "Item/Weapon")]

public class WeaponSO : Item
{


    [Header("Weapon Damage Range")]
    [SerializeField] public float damageMin;
    [SerializeField] public float damageMax;

}
