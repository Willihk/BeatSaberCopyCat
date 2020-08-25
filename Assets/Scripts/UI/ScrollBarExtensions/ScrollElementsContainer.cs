using System;
using UnityEngine;

namespace BeatGame.UI.ScrollExtensions
{
    public class ScrollElementsContainer : MonoBehaviour
    {
        public Action OnContainerChildrenChanged;

        private void OnTransformChildrenChanged()
        {
            if (OnContainerChildrenChanged != null)
            {
                OnContainerChildrenChanged.Invoke();
            }
        }
    }
}