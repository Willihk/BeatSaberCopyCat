using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;

    public UnityEvent<TabButton> OnTabSelection;

    [SerializeField]
    TabButton selectedTab;

    private void Awake()
    {
        if (OnTabSelection == null)
            OnTabSelection = new UnityEvent<TabButton>();

        if (tabButtons == null)
            tabButtons = new List<TabButton>();
        else
        {
            foreach (TabButton tabButton in tabButtons)
            {
                tabButton.SetTabGroup(this);
                tabButton.SetColor(UIPointerEvent.Idle);
            }

            if (tabButtons.Count > 0)
                OnTabSelected(tabButtons[0]);
        }
    }

    public void SelectLeft()
    {
        int targetIndex = tabButtons.IndexOf(selectedTab) - 1;
        if (targetIndex < 0)
            targetIndex = tabButtons.Count - 1;
        OnTabSelected(tabButtons[targetIndex % tabButtons.Count]);

        if (!selectedTab.gameObject.activeSelf)
            SelectLeft();
    }

    public void SelectRight()
    {
        OnTabSelected(tabButtons[(tabButtons.IndexOf(selectedTab) + 1) % tabButtons.Count]);

        if (!selectedTab.gameObject.activeSelf)
            SelectRight();
    }

    public void Subscribe(TabButton tabButton)
    {
        if (!tabButtons.Contains(tabButton))
        {
            tabButtons.Add(tabButton);
            tabButton.SetColor(UIPointerEvent.Idle);
        }
    }

    public void Unbscribe(TabButton tabButton)
    {
        if (tabButtons.Contains(tabButton))
        {
            tabButtons.Remove(tabButton);
        }
    }

    public void OnTabEnter(TabButton tabButton)
    {
        ResetTabs();

        if (tabButton != selectedTab)
            tabButton.SetColor(UIPointerEvent.Hover);
    }

    public void OnTabExit(TabButton tabButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton tabButton)
    {
        if (selectedTab != null)
            selectedTab.Deselect();

        selectedTab = tabButton;
        selectedTab.Select();

        ResetTabs();
        tabButton.SetColor(UIPointerEvent.Selected);
    }

    public void ResetTabs()
    {
        foreach (TabButton tabButton in tabButtons)
        {
            if (tabButton != selectedTab)
                tabButton.SetColor(UIPointerEvent.Idle);
        }
    }
}
