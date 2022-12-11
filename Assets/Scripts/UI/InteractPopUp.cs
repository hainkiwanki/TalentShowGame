using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractPopUp : UIElement
{
    [SerializeField]
    private TextMeshProUGUI m_interactMessageText;

    public void ShowInteract(string _interactMessage)
    {
        SetInteractMessage(_interactMessage);
        Show(0.2f);
    }

    public void SetInteractMessage(string _msg)
    {
        m_interactMessageText.text = _msg;
    }
}
