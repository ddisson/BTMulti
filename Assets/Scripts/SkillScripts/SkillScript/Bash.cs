using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bash", menuName = "Skill/Bash")]
public class Bash : AttackingSkill
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

    //public override void EffectOnCast()
    //{
    //    Debug.Log("Attacking skill effect on cast");

    //}

    public override void EffectOnAttack()
    {
        Debug.Log("If hit enemy lose 1-2 AP");
    }
}
