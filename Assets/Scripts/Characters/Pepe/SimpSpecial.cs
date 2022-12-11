using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpSpecial : MonoBehaviour
{
    protected ModSimp m_modSimp;

    public void Initialize(ModSimp _s)
    {
        m_modSimp = _s;
    }

    public abstract void Cast(string _option);
}
