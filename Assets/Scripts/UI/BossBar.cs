using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossBar : UIElement
{
    [SerializeField]
    private Image currentHpImage;
    [SerializeField]
    private TextMeshProUGUI currentHpText;
    [SerializeField]
    private TextMeshProUGUI bossName;

    public string name => bossName.text;

    public void ShowBar(string _name)
    {
        bossName.text = _name;
        Show();
    }

    public void SetHealth(float _currentHealth, float _maxHealth, float _time = 0.2f)
    {
        float currentHealth = Mathf.Clamp(_currentHealth, 0.0f, _maxHealth);
        currentHpImage.DOFillAmount(currentHealth / _maxHealth, _time);
        currentHpText.text = (currentHealth).ToString("F0");
        currentHpText.transform.DOPunchScale(Vector3.one * 1.5f, 0.2f);
    }
}
