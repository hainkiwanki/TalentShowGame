using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    public TMP_InputField authCodeInput, channelNameInput;
    private string m_oauthCode, m_channelName;
    private TwitchClient client;
    public Button connectButton, disconnectButton;

    private void Awake()
    {
        client = FindObjectOfType<TwitchClient>();
        AssingEventsToInput();
        CheckPlayerPrefs();

    }

    private void OnEnable()
    {
        EventManager.onTwitchConnection += OnTwitchConnectionChanged;
    }

    private void OnDisable()
    {
        EventManager.onTwitchConnection -= OnTwitchConnectionChanged;        
    }

    private void OnTwitchConnectionChanged(bool _isConnected)
    {
        connectButton.gameObject.SetActive(!_isConnected);
        disconnectButton.gameObject.SetActive(_isConnected);
        connectButton.interactable = true;
        disconnectButton.interactable = true;
    }

    private void CheckPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(EPlayerPrefParams.auth_code.ToString()) &&
             PlayerPrefs.HasKey(EPlayerPrefParams.channel_name.ToString()))
        {
            m_oauthCode = PlayerPrefs.GetString(EPlayerPrefParams.auth_code.ToString());
            authCodeInput.SetTextWithoutNotify(m_oauthCode);
            m_channelName = PlayerPrefs.GetString(EPlayerPrefParams.channel_name.ToString());
            channelNameInput.SetTextWithoutNotify(m_channelName);

            if(!string.IsNullOrEmpty(m_oauthCode) && !string.IsNullOrEmpty(m_channelName))
                client.ManualStart(m_channelName, m_oauthCode);
        }
    }

    private void AssingEventsToInput()
    {
        authCodeInput.onValueChanged.AddListener(OnAuthCodeChange);
        channelNameInput.onValueChanged.AddListener(OnChannelNameChange);
    }

    private void OnAuthCodeChange(string _value)
    {
        int startIndex = _value.IndexOf(':');
        if (startIndex != -1)
        {
            m_oauthCode = _value.Substring(startIndex + 1, _value.Length - startIndex - 1);
        }
        else
        {
            m_oauthCode = _value;
        }
        SavePrefs();
    }

    private void OnChannelNameChange(string _value)
    {
        m_channelName = _value;
        SavePrefs();
    }

    private void OnCommandChange(string _value)
    {
        if(string.IsNullOrEmpty(_value))
        {
            UIManager.Inst.warningMessage.ShowMessage("You cannot use empty string", 2.0f);
            return;
        }
        SavePrefs();
    }

    private void SavePrefs()
    {
        PlayerPrefs.SetString(EPlayerPrefParams.auth_code.ToString(), m_oauthCode);
        PlayerPrefs.SetString(EPlayerPrefParams.channel_name.ToString(), m_channelName);
    }

    public void OnGetAuthCode()
    {
        Application.OpenURL("https://twitchapps.com/tmi/");
    }

    public void OnConnect()
    {
        connectButton.interactable = false;
        StartCoroutine(CheckConnection(2.0f, () =>
        {
            connectButton.interactable = !client.isConnected;
        }));
    }

    public void OnDisconnect()
    {
        client.ManualDisable();
        disconnectButton.interactable = false;
    }

    IEnumerator CheckConnection(float _time, UnityAction _onComplete)
    {
        yield return new WaitForSecondsRealtime(_time);
    }
}