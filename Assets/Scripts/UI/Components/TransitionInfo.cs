using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace BeatGame.UI.Components
{
    [Serializable]
    public class TransitionInfo
    {
        public Graphic TargetGraphic;
        public Color IdleColor;
        public Color HoverColor;
        public Color SelectedColor;
    }
}