using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Attacking,
    Blocking,
    Passive
}

public enum SkillClass
{
    Basic,
    Barbarian,
    Paladin
}

//[CreateAssetMenu(fileName = "New skill", menuName = "Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public SkillClass skillClass;
    public string description;
    public Sprite skillIcon;
    public SkillType skillType;
    public int APCost;
    public int cd;
    public int numberOfTargets;
    public int leveleRequirements;
    

    public virtual void EffectOnGlobal()
    {
        return;
    }

    public virtual void EffectOnCast()
    {
        return;
    }

    public virtual void EffectPassive()
    {
        return;
    }

    public virtual void EffectPassiveTargeted(Titan titan)
    {
        return;
    }

    public virtual void EffectOnBlock()
    {
        Debug.Log("basic class block");
    }

    public virtual void EffectOnAttack()
    {
        Debug.Log("basic class attack");
    }



}
