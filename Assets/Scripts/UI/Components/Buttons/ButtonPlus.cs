using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using BeatGame.Data;

namespace BeatGame.UI.Components.Buttons
{
    public class ButtonPlus : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        public UnityEvent OnClick;

        public TransitionInfo[] Transitions;

        float pressedTime;

        private void Awake()
        {
            if (OnClick == null)
                OnClick = new UnityEvent();
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
                    case UIPointerEvent.Pressed:
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
            SetState(UIPointerEvent.Pressed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetState(UIPointerEvent.Hover);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetState(UIPointerEvent.Idle);
        }
    }
}