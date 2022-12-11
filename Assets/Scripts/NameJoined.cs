using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class NameJoined : MonoBehaviour
{
    public Image bgColorImage;
    public Image modIcon;
    public TextMeshProUGUI nameText;
    private RectTransform rectTransform;

    public void Hide()
    {
        DOTween.Kill(rectTransform);
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.zero;
    }

    public void Show(string _name, bool _isMod = false)
    {
        Hide();

        nameText.text = _name;
        modIcon.gameObject.SetActive(_isMod);

        rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => 
        {
            rectTransform.DOShakeRotation(1.0f, 10, 1).SetUpdate(true).SetLoops(-1);
        });
    }
}
