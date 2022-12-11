using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SplashNumber : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    public void Show(float _dmg)
    {
        transform.localScale = Vector3.zero;
        text.DOFade(1.0f, 0.01f);

        text.text = _dmg.ToString("F0");
        transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuint).OnComplete(() => {
            transform.DOMoveY(transform.position.y + 3.0f, 1.0f).SetEase(Ease.OutQuint);
            text.DOFade(0.0f, 0.1f).SetDelay(0.5f).OnComplete(() => {
                Destroy(gameObject);
            });
        });
    }
}
