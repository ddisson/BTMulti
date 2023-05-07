using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block Debuff", menuName = "Skill /Block Debuff")]
public class BlockDebuff: BlockingSkill
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

    public override void EffectOnBlock()
    {
        Debug.Log("I blocked and this is blockdebuff from blocking skill");
    }


    //public override void EffectOnAttack()
    //{
    //    Debug.Log("ITS A WW BABY!");
    //}
}
