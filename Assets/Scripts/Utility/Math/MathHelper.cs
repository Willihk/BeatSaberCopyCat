using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatGame.Utility.Math
{
    public static class MathHelper
    {
        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            var m = (toMax - toMin) / (fromMax - fromMin);
            var c = toMin - (m * fromMin);

            return m * value + c;
        }

        public static float RoundDecimalPlaces(this float value, int decimalPlaces)
        {
            float multiplier = Mathf.Pow(10f, decimalPlaces);
            return Mathf.Round(value * multiplier) / multiplier;
        }
    }
}
