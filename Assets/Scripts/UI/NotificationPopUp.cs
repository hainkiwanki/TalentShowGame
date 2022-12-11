using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationPopUp : UIElement
{
    [HideInInspector]
    public NotificationManager manager;
    [SerializeField]
    private TextMeshProUGUI message;

    public void ShowNotif(string _msg)
    {
        message.text = _msg;
        Show(1.0f);
        StartCoroutine(CO_DelayHide());
    }

    protected override void OnHide()
    {
        if(manager != null)
            manager.AddNotificationPopUp(this);
    }

    IEnumerator CO_DelayHide()
    {
        yield return new WaitForSeconds(3.0f);
        Hide();
    }
}
