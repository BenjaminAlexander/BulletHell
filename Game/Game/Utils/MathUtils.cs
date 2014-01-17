using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Utils
{
    static class MathUtils
    {
        public static float ClosestInRange(float value, float upper, float lower)
        {
            if (value < lower)
            {
                return lower;
            }
            if (value > upper)
            {
                return upper;
            }
            return value;
        }
    }
}
