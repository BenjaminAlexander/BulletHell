using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.Geometry
{
    public class Circle
    {
        private Vector2 center;
        private float radius;

        public Vector2 Center
        {
            get { return center; }
        }

        public float Radius
        {
            get { return radius; }
        }

        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public Boolean Intersects(Circle other)
        {
            return Vector2.Distance(other.center, this.center) < this.radius + other.radius;
        }

        public Boolean Contains(Vector2 point)
        {
            return Vector2.Distance(point, this.center) <= this.radius;
        }

        public Boolean Contains(Rectangle rectangle)
        {
            bool contains = true;
            foreach (Vector2 corner in Utils.GetCorners(rectangle))
            {
                contains = contains && this.Contains(corner);
            }
            return contains;
        }

        public static Circle CreateBounding(Rectangle rectangle)
        {
            Vector2 position = new Vector2(rectangle.X, rectangle.Y);
            Vector2 size = new Vector2(rectangle.Width, rectangle.Height);

            return new Circle(position + size / 2, (float)(Vector2.Distance(size, new Vector2(0)) / 2));
        }

    }
}
