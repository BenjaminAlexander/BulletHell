using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace MyGame.Geometry
{
    class Line
    {
        private Vector2 point1;
        private Vector2 point2;

        public Line(Vector2 point1, Vector2 point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        public Rectangle BoundingRectangle()
        {
            int x1, x2, y1, y2;

            if (point1.X > point2.X)
            {
                x1 = (int)Math.Round(point2.X);
                x2 = (int)Math.Round(point1.X - point2.X);
            }
            else
            {
                x1 = (int)Math.Round(point1.X);
                x2 = (int)Math.Round(point2.X - point1.X);
            }

            if (point1.Y > point2.Y)
            {
                y1 = (int)Math.Round(point2.Y);
                y2 = (int)Math.Round(point1.Y - point2.Y);
            }
            else
            {
                y1 = (int)Math.Round(point1.Y);
                y2 = (int)Math.Round(point2.Y - point1.Y);
            }

            return new Rectangle(x1, y1, x2, y2);
        }
    }
}
