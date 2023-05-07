using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Holy Shield", menuName = "Skill /Holy Shield")]
public class HolyShield: PassiveSkill
{
    //[SerializeField] private float damageModifier;

    //public float SkillDamageModifier(float damage)
    //{
    //    damage *= damageModifier;
    //    return damage;
    //}

    //public override void EffectOnChoose()
    //{
    //    Debug.Log("Attacking skill effect on choose");

    //}

    public override void EffectPassive()
    {
        Debug.Log("increase your shiled block chance to armor");
    }


    //public override void EffectOnAttack()
    //{
    //    Debug.Log("ITS A WW BABY!");
    //}
}
