using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TwitchConnectionIndicator : MonoBehaviour
{
    public GameObject connectButton;
    public GameObject isConnectedContainer, isNotConnectedContainer;
    public Image twitchIcon;

    private void OnEnable()
    {
        EventManager.onTwitchConnection += OnTwitchConnectionChange;
    }

    private void OnDisable()
    {
        EventManager.onTwitchConnection -= OnTwitchConnectionChange;        
    }

    private void OnTwitchConnectionChange(bool _b)
    {
        connectButton.SetActive(!_b);
        isConnectedContainer.SetActive(_b);
        isNotConnectedContainer.SetActive(!_b);

        float timer = 0.2f;
        if (_b)
        {
            twitchIcon.DOColor(new Color(0.39f, 0.26f, 0.65f, 1.0f), timer);
        }
        else
        {
            twitchIcon.DOColor(new Color(0.15f, 0.15f, 0.15f, 1.0f), timer);
        }
        twitchIcon.transform.DOPunchScale(Vector3.one * 0.2f, timer);
    }
}
