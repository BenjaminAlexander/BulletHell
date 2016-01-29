using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.Utils
{
    public static class RandomUtils
    {
        private static Random rand = new Random();

        public static Vector2 RandomVector2(Vector2 bound)
        {
            return new Vector2((float)(rand.NextDouble() * bound.X), (float)(rand.NextDouble() * bound.Y));
        }

        public static Vector2 RandomVector2(Rectangle bound)
        {
            return RandomVector2(new Vector2(bound.Width, bound.Height)) + new Vector2(bound.X, bound.Y);
        }


    }
}
