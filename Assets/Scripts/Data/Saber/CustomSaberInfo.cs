using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BeatGame.Data.Saber
{
    [Serializable]
    public class CustomSaberInfo
    {
        public GameObject SaberObject;
        public SaberDescriptor SaberDescriptor;
        public string Path;

        public override bool Equals(object obj)
        {
            return obj is CustomSaberInfo info &&
                   info.SaberDescriptor.SaberName == SaberDescriptor.SaberName &&
                   Path == info.Path;
        }
    }
}