using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attributes
{
    Strength,
    Dexterity,
    Vitality
}

[System.Serializable]
public class Attribute : Stat
{
    Attributes attribute;

}

