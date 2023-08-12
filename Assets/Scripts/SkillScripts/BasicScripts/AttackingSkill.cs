using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Atacking", menuName = "SkillType/Attacking" )]

public class AttackingSkill : SkillSO
{

    [SerializeField] private float damageModifier;

    public float SkillDamageModifier(float damage)
    {
        damage *= damageModifier;
        return damage;
    }

    public override void EffectOnGlobal()
    {
        Debug.Log("Attacking skill basic effect on choose");

    }

    public override void EffectOnCast()
    {
        Debug.Log("Attacking skill basic effect on cast");

    }

    public override void EffectOnAttack()
    {
        Debug.Log("Attacking skill basic effect on attack");
    }

    //public float GetDamage()
    //{
    //    return damage;
    //}

}

