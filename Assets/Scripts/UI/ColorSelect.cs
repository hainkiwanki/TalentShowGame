using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ColorSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Image image;
    private ColorSelector colorSelector;
    public Image selectedImg;

    public Color color
    {
        get
        {
            return image.color;
        }
        set
        {
            image.color = value;
        }
    }

    public void Initialize(ColorSelector _selector, Color _c)
    {
        image = GetComponent<Image>();
        colorSelector = _selector;
        image.color = _c;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOPunchScale(Vector3.one * 0.5f, 0.1f).SetUpdate(true);
        colorSelector.OnColorSelect(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InBack).SetUpdate(true);
        colorSelector.OnColorExit(this);
    }
}