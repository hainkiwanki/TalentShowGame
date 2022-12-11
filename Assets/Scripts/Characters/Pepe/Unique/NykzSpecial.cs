using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NykzSpecial : SimpSpecial
{
    public override void Cast(string _option)
    {
        var barrelGo = PoolManager.Inst.GetObject(EPoolObjectType.BARREL_BOMB);
        barrelGo.SetActive(true);
        BarrelBomb barrelBomb = barrelGo.GetComponent<BarrelBomb>();
        barrelBomb.Initialize(GameManager.Inst.playerControl.transform.position);
    }
}
