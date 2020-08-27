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
        public Color IdleColor = new Color(1,1,1,1);
        public Color HoverColor = new Color(0.85f, 0.85f, 0.85f, 0.85f);
        public Color SelectedColor = new Color(0.73f, 0.73f, 0.73f, 0.73f);
    }
}