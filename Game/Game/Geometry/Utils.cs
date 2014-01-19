using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.Geometry
{
    static class Utils
    {
        public static List<Vector2> GetCorners(Rectangle rectangle)
        {
            List<Vector2> list = new List<Vector2>();
            list.Add(new Vector2(rectangle.X, rectangle.Y));
            list.Add(new Vector2(rectangle.X, rectangle.Y + rectangle.Height));
            list.Add(new Vector2(rectangle.X + rectangle.Width, rectangle.Y));
            list.Add(new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height));
            return list;
        }
    }
}
