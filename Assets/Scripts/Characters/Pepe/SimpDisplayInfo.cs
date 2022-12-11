using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SimpDisplayInfo : PoolObject
{
    public RectTransform rectTrans;
    public Simp target;
    public TextMeshProUGUI nameText;
    public Image healthDisplay;
    [SerializeField]
    private SplashNumber splashNumberPrefab;
    [SerializeField]
    private RectTransform splashNumberParent;
    private CanvasGroup groupAlpha;
    public float yOffset = 40.0f;

    private void Awake()
    {
        rectTrans.localScale = Vector3.zero;
        groupAlpha = GetComponent<CanvasGroup>();
    }

    private void Update()
    {

        if(target != null)
        {
            Camera cam = GameManager.Inst.GetActiveCam();
            Vector3 screenPos = cam.WorldToScreenPoint(target.transform.position);
            Vector2 pos = new Vector2(
                screenPos.x, 
                screenPos.y + yOffset);
            rectTrans.position = pos;
        }
    }

    public void Show(bool _transparent = false)
    {
        rectTrans.DOScale(1.0f, 0.2f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        rectTrans.DOScale(0.0f, 0.2f).SetEase(Ease.InBack).OnComplete(() => 
        { 
            TurnOff();
        });
    }

    public void ShowDamage(float _dmg)
    {
        SplashNumber number = Instantiate(splashNumberPrefab, splashNumberParent);
        number.Show(_dmg);
    }

    public void SetHealth(float _normalizedHealth)
    {
        healthDisplay.fillAmount = _normalizedHealth;
    }
}
