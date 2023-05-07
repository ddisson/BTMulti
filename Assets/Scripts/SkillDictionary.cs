using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDictionary : MonoBehaviour
{

    public Dictionary<string, SkillSO> skillsDictionary = new Dictionary<string, SkillSO>();
    //public Dictionary<string, int> testDictionary = new Dictionary<string, int>();
    public List <SkillSO> skills;

    private void Awake()
    {
        //for (int i = 0; i < skills.Count; i++)
        //{
        //    skillsDictionary.Add(skills[i].name, skills[i]);
        //} 

        foreach (var skill in skills)
        {
            skillsDictionary.Add(skill.name, skill);
        }

        Debug.Log(skillsDictionary["Basic Attack"]);


        //foreach (var skill in skills)
        //{
        //    skillsDictionary.Add(skill.name, skill);
        //    Debug.Log(skillsDictionary.Values);
        //}


    }

    private void Start()
    {


        //foreach (var item in skillsDictionary)
        //{
        //    Debug.Log($"{item.Key}: {item.Value}");
        //}

        //foreach (var skill in skills)
        //{
        //    Debug.Log(skill.name);
        //}
    }


}