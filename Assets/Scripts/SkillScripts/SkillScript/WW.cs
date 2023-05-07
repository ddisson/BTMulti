using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WW", menuName = "Skill/WW")]
public class WW : AttackingSkill
{
    //[SerializeField] private float damageModifier;

    //public float SkillDamageModifier(float damage)
    //{
    //    damage *= damageModifier;
    //    return damage;
    //}

    public override void EffectOnGlobal()
    {
        Debug.Log("WW skill effect on choose");

    }

    public override void EffectOnCast()
    {
        Debug.Log("No counterattack");

    }

    public override void EffectOnAttack()
    {
        Debug.Log("ITS A WW BABY!");
    }
}
