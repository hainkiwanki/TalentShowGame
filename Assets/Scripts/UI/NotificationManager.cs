using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NotificationManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_notificationQueueParent;
    [SerializeField]
    private RectTransform m_notificationBankParent;

    private List<NotificationPopUp> m_availableNotifications;
    private Queue<string> m_notificationMessageQueue;

    private void Awake()
    {
        m_notificationMessageQueue = new Queue<string>();
        m_availableNotifications = new List<NotificationPopUp>();

        foreach (RectTransform rt in m_notificationBankParent)
        {
            NotificationPopUp popUp = rt.GetComponent<NotificationPopUp>();
            popUp.manager = this;
            popUp.Hide(0.1f);
        }
    }

    [Button]
    public void AddNotif()
    {
        ShowNotification("Notif " + m_availableNotifications.Count);
    }

    public void ShowNotification(string _notif)
    {
        m_notificationMessageQueue.Enqueue(_notif);
    }

    public void AddNotificationPopUp(NotificationPopUp _popUp)
    {
        if (!m_availableNotifications.Contains(_popUp))
            m_availableNotifications.Add(_popUp);

        _popUp.transform.parent = m_notificationBankParent;

        foreach(RectTransform rt in m_notificationQueueParent)
        {
            float endValue = rt.anchoredPosition.y + 74.0f;
            rt.DOAnchorPosY(endValue, 0.1f);
        }
    }

    public void Update()
    {
        if(m_notificationMessageQueue.Count > 0 && m_availableNotifications.Count > 0)
        {
            string msg = m_notificationMessageQueue.Dequeue();
            NotificationPopUp popup = m_availableNotifications[0];
            m_availableNotifications.RemoveAt(0);

            popup.transform.parent = m_notificationQueueParent;
            popup.rectTransform.anchoredPosition = new Vector2(popup.rectTransform.anchoredPosition.x, -(m_notificationQueueParent.childCount - 1) * 74.0f);
            popup.ShowNotif(msg);
        }
    }
}
