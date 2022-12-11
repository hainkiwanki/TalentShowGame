using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunModifierWindow : UIElement
{
    [SerializeField]
    private RectTransform modifierParent;
    private List<RunModifierElement> m_modifierList;

    private List<int> m_selectedMods;
    [SerializeField]
    private TextMeshProUGUI m_totalGoldMultiplierText;

    public List<TwitchSettingElement> twitchSettings;

    public Slider twitchTimeoutSlider;
    public TextMeshProUGUI twitchTimeoutValue;

    [FMODUnity.EventRef]
    public string onToggleOnSFX, onToggleOffSFX;

    public void Intitialize(SaveData _data)
    {
        GAMESTATS.ResetPlayerStats();
        m_modifierList = new List<RunModifierElement>();
        m_selectedMods = _data.gameData.currentRun.selectedRunMods;

        foreach (RectTransform rt in modifierParent)
        {
            RunModifierElement mod = rt.GetComponent<RunModifierElement>();
            m_modifierList.Add(mod);
        }

        for(int i = 0; i < m_modifierList.Count; i++)
        {
            m_modifierList[i].Initialize(m_selectedMods);
        }

        foreach(var setting in twitchSettings)
        {
            setting.Initialize(_data.gameData.twitchRunSettings);
        }

        twitchTimeoutValue.text = _data.gameData.twitchTimeoutTime.ToString("F0") + "s";
        twitchTimeoutSlider.value = _data.gameData.twitchTimeoutTime;
        twitchTimeoutSlider.interactable = _data.gameData.twitchRunSettings.Contains(ETwitchSettings.TIMEOUT);
        twitchTimeoutSlider.onValueChanged.AddListener(OnTimeoutSliderChanged);
    }

    public void UpdateGoldmultiplier(float _value)
    {
        Color orgColor = new Color(0.15f, 0.15f, 0.15f, 1.0f);
        m_totalGoldMultiplierText.color = orgColor;
        m_totalGoldMultiplierText.rectTransform.rotation = Quaternion.identity;

        m_totalGoldMultiplierText.DOColor(Color.green, 0.2f).SetUpdate(true).SetEase(Ease.Flash).OnComplete(() =>
        {
            m_totalGoldMultiplierText.color = orgColor;
        });
        m_totalGoldMultiplierText.rectTransform.DOPunchRotation(Vector3.forward, 0.1f).SetUpdate(true);

        m_totalGoldMultiplierText.text = "+" + _value.ToString("F0") + "%";
    }

    public void OnTimeoutToggleChanged(bool _b)
    {
        twitchTimeoutSlider.interactable = _b;
    }

    public void OnToggleTwitchSetting(bool _b)
    {
        if (_b)
            FMODUnity.RuntimeManager.PlayOneShot(onToggleOnSFX);
        else
            FMODUnity.RuntimeManager.PlayOneShot(onToggleOffSFX);
    }

    private void OnTimeoutSliderChanged(float _v)
    {
        SaveData.current.gameData.twitchTimeoutTime = _v;
        twitchTimeoutValue.text = _v.ToString("F0") + "s";
    }
}
