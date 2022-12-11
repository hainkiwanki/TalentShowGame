using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class Hainkiwanki : Singleton<Hainkiwanki>
{
    [Header("TextToSay")]
    public TextMeshProUGUI textBox;
    public RectTransform container;
    private bool isHidden = true;

    private void Awake()
    {
        container.anchoredPosition = new Vector2(0.0f, -150.0f);
    }

    public void Say(string _msg, Color _col)
    {
        if (isHidden)
        {
            isHidden = false;
            container.DOAnchorPosY(0.0f, 0.1f).SetUpdate(true).OnComplete(() =>
            {
                container.DOAnchorPosY(-150.0f, 0.1f).SetUpdate(true).OnComplete(() => { isHidden = true; }).SetDelay(3.0f);
            });
        }
        textBox.color = _col;
        textBox.text = _msg;
    }
}
