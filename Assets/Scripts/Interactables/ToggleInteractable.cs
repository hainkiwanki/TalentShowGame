using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ToggleInteractable : Interactable
{
    [GUIColor("@iData.isToggled ? new UnityEngine.Color(0.22f,0.61f,0.22f,1) : new UnityEngine.Color(0.61f,0.22f,0.22f,1)"),
        PropertyOrder(5)]
    public BoolEvent onToggle;

    public bool defaultValue = false;

    [OnValueChanged("ChangeSuffixMessage")]
    public string turnOnStr = "on";
    [OnValueChanged("ChangeSuffixMessage")]
    public string turnOffStr = "off";

    [ExecuteInEditMode]
    private void OnValidate()
    {
        ChangeSuffixMessage();
    }

    private void ChangeSuffixMessage()
    {
        suffixMessage = turnOnStr + "/" + turnOffStr;
    }

    protected override void OnUse()
    {
        iData.isToggled = !iData.isToggled;
        onToggle?.Invoke(iData.isToggled);
    }

    public override string GetInteractMessage()
    {
        return interactMessage + " " + ((iData.isToggled) ? turnOffStr : turnOnStr);
    }

    public override void PreLoad()
    {
        base.PreLoad();
        if (iData.progress == 0)
            iData.isToggled = defaultValue;

        onToggle?.Invoke(iData.isToggled);
    }
}
