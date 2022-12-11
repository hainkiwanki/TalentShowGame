using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModSimp : Simp
{
    public SimpSpecial specialAttack;

    protected override void OnSimpInitialize()
    {
        modParticle.SetActive(false);
        m_animator.gameObject.SetActive(false);
        GameManager.Inst.ExecuteFunctionWithDelay(1.5f, () => 
        {
            modParticle.SetActive(true);
            m_animator.gameObject.SetActive(true);
            if (!isTransparent)
                displayInfo.Show();
        });
        GameManager.Inst.ExecuteFunctionWithDelay(2.5f, () =>
        {
            hasSpawned = true;
        });
        specialAttack.Initialize(this);
    }

    public virtual void UseSpecialPower(string _option)
    {
        specialAttack.Cast(_option);
    }
}
