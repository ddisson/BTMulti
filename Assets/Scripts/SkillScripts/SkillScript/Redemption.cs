using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Redemption", menuName = "Skill /Redemption")]
public class Redemption : PassiveSkill
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

    public void EffectOnBlock()
    {
        Debug.Log("strange block");
    }


    //public override void EffectOnAttack()
    //{
    //    Debug.Log("ITS A WW BABY!");
    //}
}
