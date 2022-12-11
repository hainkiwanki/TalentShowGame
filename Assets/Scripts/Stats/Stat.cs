using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    private float m_baseValue = 0.0f;
    private float m_actualValue = 0.0f;

    private float m_minValue, m_maxValue;

    public float value
    {
        get
        {
            if (m_updateValue)
                UpdateValue();

            m_actualValue = Mathf.Clamp(m_actualValue, m_minValue, m_maxValue);
            return m_actualValue;
        }
    }

    public float baseValue => m_baseValue;
    private List<StatModifier> m_modifiers;
    private bool m_updateValue = true;

    public List<StatModifier> allMods => m_modifiers;

    public Stat(float _baseValue) : this(_baseValue, float.MinValue, float.MaxValue) { }

    public Stat(float _baseValue, float _min, float _max)
    {
        m_baseValue = _baseValue;
        m_modifiers = new List<StatModifier>();
        m_minValue = _min;
        m_maxValue = _max;
    }

    public void AddModifier(StatModifier _mod)
    {
        m_modifiers.Add(_mod);
        // m_modifiers.Sort();
        m_updateValue = true;
    }

    public void RemoveModifier(StatModifier _mod)
    {
        if(m_modifiers.Contains(_mod))
        {
            m_modifiers.Remove(_mod);
            m_updateValue = true;
            return;
        }

        var result = m_modifiers.Find((StatModifier _m) =>
        {
            return (_m.amt == _mod.amt && _m.type == _mod.type);
        });
        if(result == null)
        {
            Debug.LogError("Cannot remove mod [" + _mod.type.ToString() + ", " + _mod.amt.ToString("F1") + "]");
        }
        else
        {
            m_modifiers.Remove(result);
            m_updateValue = true;
        }
    }

    private void UpdateValue()
    {
        m_actualValue = m_baseValue;
        foreach(var mod in m_modifiers)
        {
            switch (mod.type)
            {
                case EStatModifierType.FLAT:
                    m_actualValue += mod.amt;
                    break;
                case EStatModifierType.PERCENT:
                    m_actualValue *= (1 + mod.amt);
                    break;
                case EStatModifierType.MULTIPLIER:
                    m_actualValue *= mod.amt;
                    break;
                case EStatModifierType.NEG_PERCENT:
                    m_actualValue *= (1 - mod.amt);
                    break;
                default:
                    break;
            }
        }
        m_updateValue = false;
    }
}
