using BeatGame.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BeatGame.UI.Tabs
{
    [Serializable]
    public class TabButtonSelected : UnityEvent<TabButton>
    {
    }

    [Serializable]
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> TabButtons;

        public TabButtonSelected OnTabSelection;

        public TabButton SelectedTab;

        private void Awake()
        {
            if (OnTabSelection == null)
                OnTabSelection = new TabButtonSelected();

            if (TabButtons == null)
                TabButtons = new List<TabButton>();
            else
            {
                foreach (TabButton tabButton in TabButtons)
                {
                    tabButton.SetTabGroup(this);
                    tabButton.SetState(UIPointerEvent.Idle);
                }

                if (TabButtons.Count > 0)
                    OnTabSelected(TabButtons[0]);
            }
        }

        public void SelectLeft()
        {
            int targetIndex = TabButtons.IndexOf(SelectedTab) - 1;
            if (targetIndex < 0)
                targetIndex = TabButtons.Count - 1;
            OnTabSelected(TabButtons[targetIndex % TabButtons.Count]);

            if (!SelectedTab.gameObject.activeSelf)
                SelectLeft();
        }

        public void SelectRight()
        {
            OnTabSelected(TabButtons[(TabButtons.IndexOf(SelectedTab) + 1) % TabButtons.Count]);

            if (!SelectedTab.gameObject.activeSelf)
                SelectRight();
        }

        public void Subscribe(TabButton tabButton)
        {
            if (!TabButtons.Contains(tabButton))
            {
                TabButtons.Add(tabButton);
                tabButton.SetState(UIPointerEvent.Idle);
            }
        }

        public void Unbscribe(TabButton tabButton)
        {
            if (TabButtons.Contains(tabButton))
            {
                TabButtons.Remove(tabButton);
            }
        }

        public void OnTabEnter(TabButton tabButton)
        {
            ResetTabs();

            if (tabButton != SelectedTab)
                tabButton.SetState(UIPointerEvent.Hover);
        }

        public void OnTabExit(TabButton tabButton)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton tabButton)
        {
            if (SelectedTab != null)
                SelectedTab.Deselect();

            SelectedTab = tabButton;
            SelectedTab.Select();

            OnTabSelection?.Invoke(tabButton);

            ResetTabs();
            tabButton.SetState(UIPointerEvent.Selected);
        }

        public void ResetTabs()
        {
            foreach (TabButton tabButton in TabButtons)
            {
                if (tabButton != SelectedTab)
                    tabButton.SetState(UIPointerEvent.Idle);
            }
        }
    }
}