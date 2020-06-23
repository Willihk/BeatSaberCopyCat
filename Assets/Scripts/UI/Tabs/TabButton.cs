using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public UnityEvent OnTabSelected;
    public UnityEvent OnTabDeselected;

    public TransitionInfo[] Transitions;

    [SerializeField]
    private TabGroup tabGroup;

    private void Awake()
    {
        if (OnTabSelected == null)
            OnTabSelected = new UnityEvent();

        if (OnTabDeselected == null)
            OnTabDeselected = new UnityEvent();
    }

    void OnEnable()
    {
        if (tabGroup != null)
            tabGroup.Subscribe(this);
    }

    public void SetTabGroup(TabGroup tabGroup)
    {
        this.tabGroup = tabGroup;
        OnEnable();
    }

    public void SetState(UIPointerEvent pointerEvent)
    {
        foreach (var item in Transitions)
        {
            switch (pointerEvent)
            {
                case UIPointerEvent.Idle:
                    item.TargetGraphic.color = item.IdleColor;
                    break;
                case UIPointerEvent.Hover:
                    item.TargetGraphic.color = item.HoverColor;
                    break;
                case UIPointerEvent.Presed:
                    break;
                case UIPointerEvent.Selected:
                    item.TargetGraphic.color = item.SelectedColor;
                    break;
                default:
                    break;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void Select()
    {
        OnTabSelected?.Invoke();
    }

    public void Deselect()
    {
        OnTabDeselected?.Invoke();
    }

    private void OnDestroy()
    {
        if (tabGroup != null)
            tabGroup.Unbscribe(this);
    }
}

[Serializable]
public class TransitionInfo
{
    public Graphic TargetGraphic;
    public Color IdleColor;
    public Color HoverColor;
    public Color SelectedColor;
}
