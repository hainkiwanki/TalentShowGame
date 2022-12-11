using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBox : UIElement
{
    [SerializeField]
    private TextMeshProUGUI messageText;
    [SerializeField]
    private TextMeshProUGUI speaker;
    [SerializeField]
    private GameObject continueIcon;

    public void ShowText(string _speaker, string _text, bool _showContinue = false)
    {
        speaker.text = _speaker;
        messageText.text = _text;
        continueIcon.SetActive(_showContinue);
        Show();
    }
}
