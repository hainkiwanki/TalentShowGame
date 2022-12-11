using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "mod", menuName = "Binki/Stats/Modifier")]
public class StatModifier : ScriptableObject, IComparable<StatModifier>
{
    public EStatModifierType type;
    public float amt = 0.0f;

    public int CompareTo(StatModifier other)
    {
        if (other == null)
            return 1;
        else
            return this.type.CompareTo(other.type);
    }
}
