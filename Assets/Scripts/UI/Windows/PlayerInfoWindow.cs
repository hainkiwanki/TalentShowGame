using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerInfoWindow : UIElement
{
    [SerializeField]
    private UIElement hpContainer, moneyContainer;

    [SerializeField]
    private TextMeshProUGUI hpText;
    [SerializeField]
    private Image hpImage;

    [SerializeField]
    private TextMeshProUGUI moneyText;

    public void SetHealth(float _currentHP, float _maxHP, bool _hide = false)
    {
        if (isHidden)
            Show();
        hpText.text = _currentHP.ToString("F1") + " / " + _maxHP.ToString("F0");
        hpImage.DOFillAmount(_currentHP / _maxHP, 0.1f);
        if (_hide)
        {
            GameManager.Inst.ExecuteFunctionWithDelayRealtime(2.0f, () => { hpContainer.Hide(); });
        }
    }

    public void SetMoneySilently(ulong _money)
    {
        moneyText.text = _money.ToString();
    }

    public void SetMoney(ulong _money, bool _hide = false)
    {
        if (isHidden)
            Show();
        moneyContainer.Show();
        moneyText.text = _money.ToString();
        if(_hide)
        {
            GameManager.Inst.ExecuteFunctionWithDelayRealtime(2.0f, () => { moneyContainer.Hide(); });
        }
    }

    public void ShowCombatUI()
    {
        if (isHidden)
            Show();
        hpContainer.Show();
        moneyContainer.Show();
    }

    public void ShowNormalUI()
    {
        if (isHidden)
            Show();
        hpContainer.Hide();
        moneyContainer.Show();
    }
}
