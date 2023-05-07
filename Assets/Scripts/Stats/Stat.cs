using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float basicValue;
    [SerializeField] private float finalValue;
    
    //public float updateValue(float modifier)
    //{

    //}


    List<float> modifiers = new List<float>();

    public void AddModifier(float modifier)
    {
        if (modifier != 0)
        {
            modifiers.Add(modifier);
        }
    }

    public float GetFinalStat()
    {
        finalValue = basicValue;
        modifiers.ForEach(x => finalValue += x);  
        return finalValue;
    }
}
