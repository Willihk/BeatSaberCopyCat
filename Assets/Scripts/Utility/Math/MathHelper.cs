using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatGame.Utility.Math
{
    public class MathHelper
    {
        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            var m = (toMax - toMin) / (fromMax - fromMin);
            var c = toMin - (m * fromMin);

            return m * value + c;
        }
    }
}
