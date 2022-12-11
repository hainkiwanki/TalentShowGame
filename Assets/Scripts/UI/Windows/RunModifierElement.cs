using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class RunModifierElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private RectTransform onHoverImage, onSelectedImage;

    [SerializeField]
    private TextMeshProUGUI runModifierTitle, runModifierDescription, runModifierNumber;
    private bool m_isToggled = false;
    public RunUpgrade runUpgrade;
    private int index;
    public RunModifierWindow runModWindow;

    [FMODUnity.EventRef]
    public string OnSelectSFX, OnDeselectSFX;

    private List<int> m_modsSelection;

    public void Initialize(List<int> _selectedMods)
    {
        runModifierTitle.text = runUpgrade.name;
        runModifierDescription.text = runUpgrade.description;
        runModifierNumber.text = "+" + (runUpgrade.goldModifer.amt * 100.0f).ToString("F0") + "%";

        m_modsSelection = _selectedMods;
        index = transform.GetSiblingIndex();
        m_isToggled = m_modsSelection.Contains(index);
        onHoverImage.localScale = new Vector3(0, 1, 1);
        onSelectedImage.gameObject.SetActive(m_isToggled);
        
        if(m_isToggled)
        {
            GAMESTATS.AddRunUpgrade(runUpgrade);
            runModWindow.UpdateGoldmultiplier(GAMESTATS.moneyPercentage * 100.0f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isToggled = !m_isToggled;
        onSelectedImage.gameObject.SetActive(m_isToggled);
        if (m_isToggled && !m_modsSelection.Contains(index))
            m_modsSelection.Add(index);
        else if(!m_isToggled && m_modsSelection.Contains(index))
            m_modsSelection.Remove(index);
        UpdateModifiers(m_isToggled);
    }

    private void UpdateModifiers(bool _b)
    {
        if (m_isToggled)
        {
            FMODUnity.RuntimeManager.PlayOneShot(OnSelectSFX);
            GAMESTATS.AddRunUpgrade(runUpgrade);
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot(OnDeselectSFX);
            GAMESTATS.RemoveRunUpgrade(runUpgrade);
        }
        runModWindow.UpdateGoldmultiplier(GAMESTATS.moneyPercentage * 100.0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverImage.DOScaleX(1.0f, 0.1f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverImage.DOScaleX(0.0f, 0.1f).SetUpdate(true);
    }
}
