using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blocking", menuName = "SkillType/Blocking")]

public class BlockingSkill : SkillSO
{

    //[SerializeField] private int blockValue;

    public override void EffectOnGlobal()
    {
        Debug.Log("block skill effect on choose");

    }

    public override void EffectOnCast()
    {
        Debug.Log("block skill effect on cast");

    }

    public override void EffectOnBlock()
    {
        Debug.Log("Block skill effect on block");
    }

}
