using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
namespace MyGame.Geometry
{
    class Triangle
    {
        private Vector2 point1;
        private Vector2 point2;
        private Vector2 point3;

        public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
        }

        public Boolean IsIntersecting(Triangle other)
        {
            if (LineIntersection(point1, point2, other.point1, other.point2) ||
                LineIntersection(point1, point2, other.point2, other.point3) ||
                LineIntersection(point1, point2, other.point3, other.point1) ||

                LineIntersection(point3, point2, other.point1, other.point2) ||
                LineIntersection(point3, point2, other.point2, other.point3) ||
                LineIntersection(point3, point2, other.point3, other.point1) ||

                LineIntersection(point1, point3, other.point1, other.point2) ||
                LineIntersection(point1, point3, other.point2, other.point3) ||
                LineIntersection(point1, point3, other.point3, other.point1))
            {
                return true;
            }

            if (other.Contains(point1) || this.Contains(other.point1))
            {
                return true;
            }

            return false;

        }

        public Boolean Contains(Vector2 point)
        {
          bool b1, b2, b3;

          b1 = Sign(point, point1, point2) < 0.0f;
          b2 = Sign(point, point2, point3) < 0.0f;
          b3 = Sign(point, point3, point1) < 0.0f;

          return ((b1 == b2) && (b2 == b3));
        }

        float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        private static bool LineIntersection(Vector2 line1Point1, Vector2 line1Point2, Vector2 line2Point1, Vector2 line2Point2/*, ref Vector2 result*/)
        {
            float a1 = line1Point1.Y - line1Point2.Y;
            float b1 = line1Point2.X - line1Point1.X;
            float c1 = a1 * line1Point2.X + b1 * line1Point2.Y;

            float a2 = line2Point1.Y - line2Point2.Y;
            float b2 = line2Point2.X - line2Point1.X;
            float c2 = a2 * line2Point2.X + b2 * line2Point2.Y;

            float det = a1 * b2 - a2 * b1;
            if (det != 0)
            {
                //lines aren't parallel 
                float x = (b2 * c1 - b1 * c2) / det;
                float y = (a1 * c2 - a2 * c1) / det;
                if (IsInRange(line1Point1.X, line1Point2.X, x) && IsInRange(line2Point1.X, line2Point2.X, x)
                    && IsInRange(line1Point1.Y, line1Point2.Y, y) && IsInRange(line2Point1.Y, line2Point2.Y, y))
                {
                    //result = new Vector2(x, y);
                    return true;
                }

            }
            return false;
        }

        private static bool IsInRange(float point1, float point2, float value)
        {
            if (point1 >= point2)
            {
                return point2 <= value && value <= point1;
            }
            else if (point1 < point2)
            {
                return point1 <= value && value <= point2;
            }
            else
            {
                return point2 == value;
            }
        }

        public void Draw(MyGraphicsClass graphics, Vector2 pos)
        {
            if (this.Contains(pos))
            {
                Draw(graphics, Color.Red, 1);
            }
            else
            {
                Draw(graphics, Color.Blue, 1);
            }
        }

        public void Draw(MyGraphicsClass graphics, Color color, float depth)
        {
            graphics.DrawLine(point1, point2, color, depth);
            graphics.DrawLine(point1, point3, color, depth);
            graphics.DrawLine(point3, point2, color, depth);
        }
    }
}
