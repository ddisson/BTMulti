using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionType { Attack, Block}

[System.Serializable]
public class Action 
{
    // Start is called before the first frame update

    public int actionIndex;
    public SkillSO skill;
    public int targetsNeeded;
    public ActionType actionType;
    public List<BodyPart> targets = new List<BodyPart>();
    //public GameObject targetImage = new GameObject();
    public List<GameObject> targetImages = new List<GameObject>();

    public Button cancelButton; // Each action has its own cancel button

    public ActionType GetActionType(SkillSO skill)
    {
        if (skill.skillType == SkillType.Attacking)
        {
            this.actionType = ActionType.Attack;
        }

        else if (skill.skillType == SkillType.Blocking)
        {
            this.actionType = ActionType.Block;
        }

        return this.actionType;
        
    }

}
