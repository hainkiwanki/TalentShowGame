using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WarningPopUp : UIElement
{
    public TextMeshProUGUI messageText;
    private Coroutine m_hideRoutine;

    public void ShowMessage(string _msg, float _timeToShow)
    {
        messageText.text = _msg;
        if (m_hideRoutine != null)
            StopAllCoroutines();
        m_hideRoutine = StartCoroutine(CO_HideAutomatically(_timeToShow));
        Show();
    }

    IEnumerator CO_HideAutomatically(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        Hide();
    }
}
