using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDB : MonoBehaviour
{
    public static SkillDB instance;

    public SkillSO[] allSkills;  // Populate this in the inspector
    public Dictionary<int, SkillSO> skillDictionary;

    void Awake()
    {
        // Singleton pattern to make sure there's only one SkillDatabase instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Populate the skill dictionary
        skillDictionary = new Dictionary<int, SkillSO>();
        foreach (SkillSO skill in allSkills)
        {
            skillDictionary.Add(skill.skillID, skill);
        }
    }
}
