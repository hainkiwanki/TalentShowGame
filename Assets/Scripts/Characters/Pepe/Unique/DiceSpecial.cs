using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSpecial : SimpSpecial
{
    public override void Cast(string _option)
    {
        if(System.Int32.TryParse(_option, out int result))
        {
            if(result == 1)
            {
                PostProcessingManager.Inst.SetPixelize(0.04f);
            }
            else if(result == 2)
            {
                PostProcessingManager.Inst.SetRipples(5.0f);
            }
            else
            {
                PostProcessingManager.Inst.SetRefraction(0.2f);
            }
        }
    }
}
