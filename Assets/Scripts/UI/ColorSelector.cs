using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum EColorCategory
{
    PRIMARY,
    SECONDARY,
    LEATHER_PRIMARY,
    LEATHER_SECONDARY,
    METAL_PRIMARY,
    METAL_SECONDARY,
    METAL_DARK,
}

public class ColorSelector : MonoBehaviour
{
    public EColorCategory colorCategory;
    public TextMeshProUGUI categoryText;
    public RectTransform gridTransform;
    public ColorSelect colorSelectPrefab;
    private RectTransform m_rectTransform;
    private float m_minHeight = 43.0f;
    private float m_rowHeight = 34.0f;
    private List<ColorSelect> colors = new List<ColorSelect>();
    private ColorSelect currentColorSelected = null;

    public PlayerComposer playerComposer;

    private void Awake()
    {
        categoryText.text = ColorCatToString();
        var colorsPerCategory = SaveData.current.playerCustomization.colorsPerCategory;
        var colorsInUse = SaveData.current.playerCustomization.colorsInUse;
        for(int i = 0; i < colorsPerCategory[colorCategory].Count; i++)
        {
            var colorSelect = Instantiate(colorSelectPrefab, gridTransform);
            colorSelect.Initialize(this, colorsPerCategory[colorCategory][i]);
            colors.Add(colorSelect);
            if (i == colorsInUse[colorCategory])
                OnColorSelect(colorSelect);
        }

        float columns = 13.0f;
        m_rectTransform = GetComponent<RectTransform>();
        m_rectTransform.sizeDelta = new Vector2(
            m_rectTransform.sizeDelta.x, 
            m_minHeight + m_rowHeight * Mathf.CeilToInt(colorsPerCategory[colorCategory].Count / columns));
    }

    private string ColorCatToString()
    {
        var result = "";
        var arr = colorCategory.ToString().Split('_');
        for(int i = 0; i < arr.Length; i++)
        {
            result += arr[i].Capitalize();
            if (i != arr.Length - 1)
                result += " ";
        }
        return result;
    }

    public void OnColorSelect(ColorSelect _c)
    {
        currentColorSelected = _c;
        ResetColorSelects();
        currentColorSelected.selectedImg.gameObject.SetActive(true);
        SaveData.current.playerCustomization.colorsInUse[colorCategory] = currentColorSelected.transform.GetSiblingIndex();
        playerComposer.SetMaterialColor(colorCategory, currentColorSelected.color);
    }

    public void OnColorExit(ColorSelect _c)
    {
        ResetColorSelects();
    }

    private void ResetColorSelects()
    {
        foreach(var color in colors)
        {
            if (color == currentColorSelected && currentColorSelected != null)
                continue;
            color.selectedImg.gameObject.SetActive(false);
        }
    }
}
