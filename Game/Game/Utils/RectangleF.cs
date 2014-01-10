using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.Utils
{
    public class RectangleF
    {
        private Vector2 position;
        private Vector2 size;

        public RectangleF(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }

        public RectangleF(Rectangle rec)
        {
            this.position = new Vector2(rec.X, rec.Y);
            this.size = new Vector2(rec.Width, rec.Height);
        }

        public bool Contains(Vector2 point)
        {
            Vector2 pointInRectangle = point - position;
            return isInRange(0, size.X, pointInRectangle.X) && isInRange(0, size.Y, pointInRectangle.Y);
        }

        private static bool isInRange(float point1, float point2, float value)
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

        /*public static bool ScreenRectangleContainWorldPoint(RectangleF rec, Camera camera, Vector2 point)
        {
            return rec.Contains(Vector2.Transform(point, camera.GetWorldToScreenTransformation()));
        }*/

        public List<Vector2> LineCollision(Vector2 point1, Vector2 point2)
        {
            List<Vector2> intersectionList = new List<Vector2>();
            Vector2 result = new Vector2(0);

            if (LineIntersection(position, new Vector2(size.X, 0) + position, point1, point2, ref result))
            {
                intersectionList.Add(result);
            }

            if (LineIntersection(position, new Vector2(0, size.Y) + position, point1, point2, ref result))
            {
                intersectionList.Add(result);
            }

            if (LineIntersection(size + position, new Vector2(0, size.Y) + position, point1, point2, ref result))
            {
                intersectionList.Add(result);
            }

            if (LineIntersection(size + position, new Vector2(size.X, 0) + position, point1, point2, ref result))
            {
                intersectionList.Add(result);
            }

            return intersectionList;
        }

        public static bool LineIntersection(Vector2 line1Point1, Vector2 line1Point2, Vector2 line2Point1, Vector2 line2Point2, ref Vector2 result)
        {
            float a1 = line1Point1.Y - line1Point2.Y;
            float b1 = line1Point2.X - line1Point1.X;
            float c1 = a1 * line1Point2.X + b1 * line1Point2.Y;

            float a2 = line2Point1.Y - line2Point2.Y;
            float b2 = line2Point2.X - line2Point1.X;
            float c2 = a2 * line2Point2.X + b2 * line2Point2.Y;

            float det = a1*b2 - a2*b1;
            if(det != 0)
            {
                //lines aren't parallel 
                float x = (b2 * c1 - b1 * c2) / det;
                float y = (a1 * c2 - a2 * c1) / det;
                if (RectangleF.isInRange(line1Point1.X, line1Point2.X, x) && RectangleF.isInRange(line2Point1.X, line2Point2.X, x)
                    && RectangleF.isInRange(line1Point1.Y, line1Point2.Y, y) && RectangleF.isInRange(line2Point1.Y, line2Point2.Y, y))
                {
                    result = new Vector2(x, y);
                    return true;
                }
                
            }
            return false;

        }

        public Vector2 ClosestPoint(Vector2 point)
        {
            if(this.Contains(point))
            {
                return point;
            }
            Vector2 apoint = point - position;

            float x = apoint.X;
            float x1 = Math.Max(0, apoint.X);
            float x2 = Math.Min(size.X, apoint.X);

            if (x1 == 0)
                x = x1;
            if (x2 == size.X)
                x = x2;

            float y = apoint.Y;
            float y1 = Math.Max(0, apoint.Y);
            float y2 = Math.Min(size.Y, apoint.Y);

            if (y1 == 0)
                y = y1;
            if (y2 == size.Y)
                y = y2;

            return new Vector2(x, y);
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 Size
        {
            get { return size; }
        }
    }
}
