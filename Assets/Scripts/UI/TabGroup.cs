using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public List<GameObject> pages;
    public Color tabIdle;
    public Color tabHover;
    public Color tabSelected;
    public TabButton selectedTab;
    public TabButton startTab;

    public void Subscribe(TabButton _button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();

        if (startTab == _button)
            OnTabSelected(_button);
        else
            OnTabExit(_button);

        tabButtons.Add(_button);
    }

    public void OnTabEnter(TabButton _b)
    {
        ResetTabs();
        if (selectedTab != null && _b != selectedTab)
            _b.background.color = tabHover;
    }

    public void OnTabExit(TabButton _b)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton _b)
    {
        selectedTab = _b;
        ResetTabs();
        _b.background.color = tabSelected;
        int index = _b.transform.GetSiblingIndex();
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
        }
    }

    public void ResetTabs()
    {
        foreach(var button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab)
                continue;
            button.background.color = tabIdle;
        }
    }
}
