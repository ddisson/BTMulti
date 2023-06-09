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
    public ActionType actionType;
    public List<BodyPart> targets = new List<BodyPart>();
    //public GameObject targetImage = new GameObject();
    public List<GameObject> targetImages = new List<GameObject>();

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

    //target contain index of image
    //it receives on click when added to image list by .count-1 index and sends it back to new target class

//using Photon.Pun;
//using System.Collections.Generic;

//public class Action : IPunObservable
//{
//    public int actionIndex;
//    public SkillSO skill;
//    public ActionType actionType;
//    public List<BodyPart> targets = new List<BodyPart>();

//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (stream.IsWriting)
//        {
//            // Serialize data to send
//            stream.SendNext(actionIndex);
//            stream.SendNext(skill.skillID); // Assuming SkillSO has a skillID property
//            stream.SendNext((int)actionType);
//            stream.SendNext(targets.Count);
//            foreach (BodyPart target in targets)
//            {
//                stream.SendNext((int)target);
//            }
//        }
//        else
//        {
//            // Deserialize data received
//            actionIndex = (int)stream.ReceiveNext();
//            int skillID = (int)stream.ReceiveNext();
//            skill = SkillSO.GetSkillByID(skillID); // Assuming you have a method to retrieve SkillSO by its ID
//            actionType = (ActionType)stream.ReceiveNext();
//            int targetCount = (int)stream.ReceiveNext();
//            targets.Clear();
//            for (int i = 0; i < targetCount; i++)
//            {
//                targets.Add((BodyPart)stream.ReceiveNext());
//            }
//        }
//    }
//}

}
