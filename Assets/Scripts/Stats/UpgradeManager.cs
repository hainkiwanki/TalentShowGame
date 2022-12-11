using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    private enum EChatVoteState
    {
        DISABLED,
        CONFIRMATION,
        ENABLED,
    }

    [SerializeField]
    private GameObject m_upgradeMenu;
    [HideInInspector]
    public bool hasSelectedUpgrade = false;
    [SerializeField]
    private Transform m_rotatinRays;

    [SerializeField]
    private List<UpgradeElement> m_upgradeElements;

    [SerializeField]
    private List<PlayerUpgrade> upgrades;
    [SerializeField]
    private TextMeshProUGUI m_upgradeDescription;

    [Header("Chat Voting")]
    [SerializeField]
    private TextMeshProUGUI m_allowVoteButtonText;
    [SerializeField]
    private RectTransform m_voteContainerRect;
    [SerializeField]
    private TextMeshProUGUI m_confirmationTurnOnText;
    [SerializeField]
    private GameObject m_cancelButton;
    [SerializeField]
    private TextMeshProUGUI m_totalVotesText;
    [SerializeField]
    private UIElement m_totalVotesContainer;
    [SerializeField]
    private Transform m_timerContainer;
    [SerializeField]
    private TextMeshProUGUI m_timerText;

    private float m_minHeight = 300.0f;
    private float m_midHeight = 490.0f;
    private float m_maxHeight = 720.0f;
    private float m_width = 300.0f;
    private bool m_isAnimating = false;
    private EChatVoteState m_chatVoteState = EChatVoteState.DISABLED;
    private bool m_isVoting = false;

    private bool shouldDisableVoteNextRound = false;

    private List<PlayerUpgrade> m_pickedUpgrades = new List<PlayerUpgrade>();
    private Dictionary<string, int> m_upgradeVotesDic;
    private PlayerUpgrade m_selectedUpgrade = null;

    private void Update()
    {
        m_rotatinRays.Rotate(Vector3.forward * 10.0f * Time.unscaledDeltaTime);
    }

    public void Show(int _levelCompleted)
    { 
        m_upgradeMenu.SetActive(true);
        if(shouldDisableVoteNextRound)
        {
            SaveData.current.chatCanVoteUpgrade = false;
        }
        SetChatVoteState((SaveData.current.chatCanVoteUpgrade) ? EChatVoteState.ENABLED : EChatVoteState.DISABLED, 0.0f);
        m_pickedUpgrades = GetRandomUpgrades();
        m_upgradeVotesDic = new Dictionary<string, int>();
        for (int i = 0; i < m_upgradeElements.Count; i++)
        {
            m_upgradeElements[i].gameObject.SetActive(m_pickedUpgrades[i] != null);
            if (m_pickedUpgrades[i] != null)
            {
                m_upgradeVotesDic.Add(m_pickedUpgrades[i].animationName, 0);
                int index = i;
                m_upgradeElements[i].Initialize(m_pickedUpgrades[i],
                    () =>
                    {
                        m_upgradeDescription.text = m_pickedUpgrades[index].description;
                    },
                    (UpgradeElement _selectedElement) =>
                    {
                        foreach (var elements in m_upgradeElements)
                        {
                            if (elements == _selectedElement)
                            {
                                elements.Select();
                                m_selectedUpgrade = elements.upgrade;
                            }
                            else
                                elements.Deselect();
                        }
                    },
                    false);
            }
        }
        hasSelectedUpgrade = false;
    }

    private List<PlayerUpgrade> GetRandomUpgrades()
    {
        List<PlayerUpgrade> list = new List<PlayerUpgrade>();

        int amountOfUpgradesNeeded = 3;

        while(list.Count < amountOfUpgradesNeeded && upgrades.Count > 0)
        {
            var upgr = upgrades.GetRandom();
            if(Extensions.ChanceRoll(100.0f - upgr.chance) || upgrades.Count == 1)
            {
                list.Add(upgr);
                upgrades.Remove(upgr);
            }
        }

        if(list.Count < amountOfUpgradesNeeded)
        {
            for(int i = 0; i < amountOfUpgradesNeeded - list.Count; i++)
            {
                list.Add(null);
            }
        }

        return list;
    }

    public void Hide()
    {
        m_upgradeMenu.SetActive(false);
    }

    public void OnChoose()
    {
        if (m_selectedUpgrade == null)
            m_selectedUpgrade = m_pickedUpgrades.GetRandom();
        foreach(var upgrade in m_pickedUpgrades)
        {
            if(upgrade != null && !upgrade.oneTimeOnly)
                upgrades.Add(upgrade);
        }
        m_pickedUpgrades.Clear();
        hasSelectedUpgrade = true;
        GAMESTATS.AddPlayerUpgrade(m_selectedUpgrade);
    }

    private void SetChatVoteState(EChatVoteState _newState, float _animTime = 0.2f)
    {
        if (m_isAnimating) return;

        switch (_newState)
        {
            case EChatVoteState.DISABLED:
                m_isAnimating = true;
                SaveData.current.chatCanVoteUpgrade = false;
                m_voteContainerRect.DOSizeDelta(new Vector2(m_width, m_minHeight), _animTime).SetUpdate(true).OnComplete(() =>
                {
                    m_cancelButton.SetActive(false);
                    m_allowVoteButtonText.text = "Turn on";
                    m_isAnimating = false;
                    m_chatVoteState = _newState;
                    m_totalVotesContainer.Hide();
                    m_timerContainer.DOScale(Vector3.zero, 0.2f).SetUpdate(true);
                });
                break;
            case EChatVoteState.CONFIRMATION:
                m_isAnimating = true;
                m_voteContainerRect.DOSizeDelta(new Vector2(m_width, m_midHeight), _animTime).SetUpdate(true).OnComplete(() =>
                {
                    m_allowVoteButtonText.text = "Yes";
                    m_cancelButton.SetActive(true);
                    m_isAnimating = false;
                    m_chatVoteState = _newState;
                    m_totalVotesContainer.Hide();
                });
                break;
            case EChatVoteState.ENABLED:
                m_isAnimating = true;
                m_voteContainerRect.DOSizeDelta(new Vector2(m_width, m_maxHeight), _animTime).SetUpdate(true).OnComplete(() =>
                {
                    m_isVoting = true;
                    m_allowVoteButtonText.text = "Turn off";
                    m_cancelButton.SetActive(false);
                    m_isAnimating = false;
                    m_chatVoteState = _newState;
                    m_totalVotesContainer.Show();
                    m_timerContainer.DOScale(Vector3.one, 0.2f).SetUpdate(true);
                    foreach (var upgradeElement in m_upgradeElements)
                        upgradeElement.EnableVoting();
                    SaveData.current.chatCanVoteUpgrade = true;
                    StartCoroutine(CO_StopCounting());
                });
                break;
            default:
                break;
        }
    }

    IEnumerator CO_StopCounting()
    {
        float timer = 60.0f;
        while(timer > 0.0f)
        {
            timer -= Time.unscaledDeltaTime;
            m_timerText.text = timer.ToString("F0");
            yield return null;
        }
        hasSelectedUpgrade = true;
        m_isVoting = false;
        yield return null;
    }

    public void OnChatVoteButton()
    {
        switch (m_chatVoteState)
        {
            case EChatVoteState.DISABLED:
                SetChatVoteState(EChatVoteState.CONFIRMATION);
                break;
            case EChatVoteState.CONFIRMATION:
                SetChatVoteState(EChatVoteState.ENABLED);
                break;
            case EChatVoteState.ENABLED:
                UIManager.Inst.warningMessage.ShowMessage("Chat voting will be turned off next round.", 3.0f);
                shouldDisableVoteNextRound = true;
                break;
            default:
                break;
        }
    }

    public void OnChatVoteCancelButton()
    {
        SetChatVoteState(EChatVoteState.DISABLED);
    }

    public void ReceiveVote(string _vote)
    {
        if (!m_isVoting) return;
        string vote = _vote.ToLower();
        if(m_upgradeVotesDic.ContainsKey(vote))
        {
            m_upgradeVotesDic[vote]++;
            UpdateTotalVotes();
        }
        else if(vote == "1" && m_upgradeVotesDic.Count > 0)
        {
            string key = m_upgradeVotesDic.ElementAt(0).Key;
            m_upgradeVotesDic[key]++;
            UpdateTotalVotes();
        }
        else if (vote == "2" && m_upgradeVotesDic.Count > 1)
        {
            string key = m_upgradeVotesDic.ElementAt(1).Key;
            m_upgradeVotesDic[key]++;
            UpdateTotalVotes();
        }
        else if (vote == "3" && m_upgradeVotesDic.Count > 2)
        {
            string key = m_upgradeVotesDic.ElementAt(2).Key;
            m_upgradeVotesDic[key]++;
            UpdateTotalVotes();
        }
    }

    private void UpdateTotalVotes()
    {
        float votes = 0;
        string mostVotes = "";
        float mostVotesAmount = float.MinValue;
        foreach (var item in m_upgradeVotesDic)
        {
            votes += item.Value;
            if(item.Value > mostVotesAmount)
            {
                mostVotes = item.Key;
                mostVotesAmount = item.Value;
            }
        }
        m_totalVotesText.text = "Votes: " + votes.ToString("F0");

        for(int i = 0; i < m_upgradeElements.Count; i++)
        {
            if (m_upgradeElements[i].upgradeAnimationName == mostVotes)
                m_upgradeElements[i].Select();
            else
                m_upgradeElements[i].Deselect();
            m_upgradeElements[i].SetPercentage((float)m_upgradeVotesDic[m_upgradeElements[i].upgradeAnimationName] / votes);
        }
    }
}
