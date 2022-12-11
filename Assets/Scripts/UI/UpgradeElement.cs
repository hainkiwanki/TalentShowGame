using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class UpgradeElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private Image m_upgradeIcon;
    [SerializeField]
    private TextMeshProUGUI m_upgradeName;
    [SerializeField]
    private GameObject m_isSelectedGO;
    [SerializeField]
    private GameObject m_isMajorUpgradeGO;
    [SerializeField]
    private TextMeshProUGUI m_percentagePicked;

    private UnityAction<UpgradeElement> m_onClickCallback;
    private UnityAction m_onHoverCallback;

    [SerializeField]
    private Animator m_animator;
    private PlayerUpgrade m_uprgade;
    public PlayerUpgrade upgrade => m_uprgade;

    private bool m_votingIsAllowed = false;
    public string upgradeAnimationName;

    public void Initialize(PlayerUpgrade _upgrade,
        UnityAction _onHover, UnityAction<UpgradeElement> _onClick, bool _isMajor = false)
    {
        m_uprgade = _upgrade;
        m_upgradeName.text = m_uprgade.name;
        m_upgradeIcon.sprite = m_uprgade.icon;
        m_isSelectedGO.SetActive(false);
        m_isMajorUpgradeGO.SetActive(_isMajor);
        m_votingIsAllowed = false;
        m_onHoverCallback = _onHover;
        m_onClickCallback = _onClick;
        m_percentagePicked.text = "0 %";
        m_percentagePicked.gameObject.SetActive(false);
        upgradeAnimationName = _upgrade.animationName;
        if (!string.IsNullOrEmpty(_upgrade.animationName) && _upgrade.hasAnimation)
        {
            m_animator.enabled = true;
            m_animator.Play(_upgrade.animationName, -1);
        }
        else
        {
            m_animator.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!m_votingIsAllowed)
            m_onClickCallback?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_onHoverCallback?.Invoke();
        transform.DOScale(1.1f, 0.1f).SetUpdate(true);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 5));
        transform.DORotate(new Vector3(0, 0, -5), 1.0f).SetUpdate(true).SetLoops(-1, LoopType.Yoyo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Kill(transform);
        transform.DOScale(1.0f, 0.1f).SetUpdate(true);
        transform.rotation = Quaternion.identity;
    }

    public void EnableVoting()
    {
        m_votingIsAllowed = true;
        m_percentagePicked.gameObject.SetActive(true);
    }

    public void Select()
    {
        m_isSelectedGO.SetActive(true);
    }

    public void Deselect()
    {
        m_isSelectedGO.SetActive(false);
    }

    public void SetPercentage(float _f)
    {
        float value = Mathf.Clamp(_f, 0.0f, 1.0f);
        m_percentagePicked.text = (value * 100.0f).ToString("F0") + " %";
    }
}
