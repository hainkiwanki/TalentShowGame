using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwitchSettingElement : MonoBehaviour
{
    [SerializeField]
    private Toggle toggleButton;
    public ETwitchSettings setting;
    private HashSet<ETwitchSettings> m_twitchSettings;

    public BoolEvent onToggleCallback;

    public void Initialize(HashSet<ETwitchSettings> _settings)
    {
        m_twitchSettings = _settings;
        toggleButton.isOn = _settings.Contains(setting);
        toggleButton.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool _b)
    {
        if(_b && !m_twitchSettings.Contains(setting))
        {
            m_twitchSettings.Add(setting);
        }
        else if(!_b && m_twitchSettings.Contains(setting))
        {
            m_twitchSettings.Remove(setting);
        }
        onToggleCallback?.Invoke(_b);
    }
}
