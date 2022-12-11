using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

public class ViewerVoteElement : MonoBehaviour
{
    [SerializeField]
    private Image m_iconImage, m_filledImage;
    [SerializeField]
    private TextMeshProUGUI m_questionMark;
    private int m_currentVotes = 0;
    private int m_neededVotes = 500;
    public string voteNumber;
    private UnityAction m_onVoteCompleted;

    public void Initialize(int _index, Sprite _sprite, UnityAction _onVoteComplete)
    {
        m_currentVotes = 0;
        voteNumber = _index.ToString();
        m_iconImage.sprite = _sprite;
        m_onVoteCompleted = _onVoteComplete;
        m_iconImage.transform.localScale = Vector3.zero;
        m_questionMark.transform.localScale = Vector3.one;
        m_filledImage.fillAmount = 0.0f;
    }

    public void SendVote(string _vote)
    {
        if (voteNumber != _vote || m_currentVotes >= m_neededVotes) return;

        m_currentVotes++;
        float ratio = (float)m_currentVotes / (float)m_neededVotes;
        m_filledImage.DOFillAmount(ratio, 0.2f);
        if(m_currentVotes >= m_neededVotes)
        {
            m_questionMark.transform.DOScale(0.0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                m_iconImage.transform.DOScale(1.0f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    m_onVoteCompleted?.Invoke();
                });
            });
        }
    }
}
