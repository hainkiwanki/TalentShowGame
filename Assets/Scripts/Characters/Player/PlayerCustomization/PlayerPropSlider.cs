using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EPlayerPropPart
{
    HANDS,
    LOWERARM,
    UPPERARM,
    CHEST,
    PANTS,
    BOOTS,
}

public class PlayerPropSlider : MonoBehaviour
{
    private Slider slider;
    public IntEvent onValueChanged;
    public EPlayerPropPart part;
    public TextMeshProUGUI indexIndicator;

    private void Awake()
    {
        InitializeSlider();
    }

    private void InitializeSlider()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderChange);
        var sliderData = SaveData.current.playerCustomization;
        switch (part)
        {
            case EPlayerPropPart.HANDS:
                slider.maxValue = sliderData.unlockedHandIndices.Count - 1;
                break;
            case EPlayerPropPart.LOWERARM:
                slider.maxValue = sliderData.unlockedLowerArmIndices.Count - 1;
                break;
            case EPlayerPropPart.UPPERARM:
                slider.maxValue = sliderData.unlockedUpperArmIndices.Count - 1;
                break;
            case EPlayerPropPart.CHEST:
                slider.maxValue = sliderData.unlockedChestIndices.Count - 1;
                break;
            case EPlayerPropPart.PANTS:
                slider.maxValue = sliderData.unlockedPantsIndices.Count - 1;
                break;
            case EPlayerPropPart.BOOTS:
                slider.maxValue = sliderData.unlockedBootsIndices.Count - 1;
                break;
            default:
                break;
        }
    }

    private void OnSliderChange(float _value)
    {
        var sliderData = SaveData.current.playerCustomization;
        int index = Mathf.RoundToInt(_value);
        if(indexIndicator != null)
        {
            indexIndicator.text = (index + 1).ToString();
        }
        switch (part)
        {
            case EPlayerPropPart.HANDS:
                onValueChanged?.Invoke(sliderData.unlockedHandIndices[index]);
                break;
            case EPlayerPropPart.LOWERARM:
                onValueChanged?.Invoke(sliderData.unlockedLowerArmIndices[index]);
                break;
            case EPlayerPropPart.UPPERARM:
                onValueChanged?.Invoke(sliderData.unlockedUpperArmIndices[index]);
                break;
            case EPlayerPropPart.CHEST:
                onValueChanged?.Invoke(sliderData.unlockedChestIndices[index]);
                break;
            case EPlayerPropPart.PANTS:
                onValueChanged?.Invoke(sliderData.unlockedPantsIndices[index]);
                break;
            case EPlayerPropPart.BOOTS:
                onValueChanged?.Invoke(sliderData.unlockedBootsIndices[index]);
                break;
            default:
                break;
        }
    }
}
