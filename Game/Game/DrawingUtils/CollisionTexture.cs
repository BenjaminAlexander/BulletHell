using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MyGame.Geometry;

namespace MyGame.DrawingUtils
{
    public class CollisionTexture
    {
        private Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
        }

        Color[] data;
        public Color[] Data
        {
            get { return data; }
        }

        private Vector2 centerOfMass;
        public Vector2 CenterOfMass
        {
            get { return centerOfMass; }
        }

        private Circle boundingCircle;
        public Circle BoundingCircle
        {
            get { return boundingCircle; }
        }

        private List<Point> border;
        public List<Point> Border
        {
            get { return border; }
        }

        // An array of offsets.
        //
        //  123
        //  0X4
        //  765
        //
        private static readonly Point[] BorderPointOffsets = {new Point(-1,0), new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(1, 0), new Point(1, 1), new Point(0, 1), new Point(-1, 1)};

        public CollisionTexture(ContentManager content, String contentName)
        {
            texture = content.Load<Texture2D>(contentName);
            data = new Color[this.texture.Width * this.texture.Height];
            this.texture.GetData(data);
            centerOfMass = ComputeCenterOfMass();
            border = ComputeBorder();
            boundingCircle = ComputeBoundingCircle();
        }

        // Computes the center of mass of the texture.
        private Vector2 ComputeCenterOfMass()
        {
            Vector2 sum = new Vector2(0);
            int count = 0;
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    if (!ZeroPixel(x, y))
                    {
                        sum = sum + new Vector2(x, y);
                        count++;
                    }
                }
            }
            centerOfMass = sum / count;
            return centerOfMass;
        }

        // Computes the bounding circle of the texture. (Useful for collision detection.) 
        private Circle ComputeBoundingCircle()
        {
            Rectangle bounds = texture.Bounds;
            Point center = bounds.Center;
            float radius = (float)Math.Sqrt(Math.Pow(bounds.Width-center.X, 2.0) + Math.Pow(bounds.Height-center.Y, 2.0));
            return new Circle(new Vector2(center.X, center.Y), radius);
        }

        private Color GetData(int x, int y)
        {
            return data[x + y * texture.Width];
        }

        // Sets an element of data.
        private void SetData(int x, int y, Color c)
        {
            data[x + y * texture.Width] = c;
        }

        // Checks to see whether or not a point is within the rectangle bounds of the texture.
        private bool InBounds(int x, int y)
        {
            return (x >= 0 && x < texture.Width && y >= 0 && y < texture.Height);
        }
        
        // Overload for Point 
        private bool InBounds(Point p)
        {
            return InBounds(p.X, p.Y);
        }

        // Checks to see if the pixel is a zero pixel (has an alpha value of 0);
        private bool ZeroPixel(int x, int y)
        {
            if (InBounds(x, y))
            {
                return GetData(x, y).A == 0;
            }

            // An out of bounds pixel is a zero-pixel.
            return true;
        }

        // Overload for Point.
        private bool ZeroPixel(Point p)
        {
            return ZeroPixel(p.X, p.Y);
        }

        // I do not trust the XNA equals method.
        private static bool PointEquals(Point a, Point b)
        {
            return (a.X == b.X && a.Y == b.Y);
        }

        // Returns the n'th adjacent clockwise point to p, where the point to the left of p is the 0'th adjacent point.
        private static Point AddOffset(Point p, uint n)
        {
            Point offset = BorderPointOffsets[n%8];
            return new Point(p.X + offset.X, p.Y + offset.Y);
        }

        // Given two adjacent points a and b, returns the number n such that a = AddOffset(b, n)
        private static uint OffsetNumber(Point a, Point b)
        {
            Point offset = new Point(a.X - b.X, a.Y - b.Y);
            uint i = 0;
            while (i < 8 && !PointEquals(BorderPointOffsets[i], offset))
            {
                i++;
            }

            if (i >= 8)
            {
                throw new Exception("The two points given were not adjacent.");
            }

            return i;
        }

        // Given a start point that is a border pixel, finds the next clockwise border pixel.
        private Point ComputeBorderHelper(List<Point> border)
        {
            // The most recently added border point.
            Point startPoint = border[border.Count - 1];
            // The previous start point (second to last)
            Point previousStartPoint;
            // The offset number from which we will begin our search for the next border point.
            uint offsetStart;
            
            // The previous start point is the previously added border point.
            if (border.Count >= 2)
            {
                previousStartPoint = border[border.Count - 2];
                // We start the search from the next clockwise point from the previous start point.
                offsetStart = OffsetNumber(previousStartPoint, startPoint) + 1;
            }
            // If there are not two points in the list (first iteration), then the previous start point will 
            // be the point directly to the left of the first start point (we know this is a zero pixel).
            else
            {
                previousStartPoint = new Point(startPoint.X - 1, startPoint.Y);

                // We start our search from the point to the left of the start point since this is our first iteration.
                offsetStart = 0;
            }

            Point nextPoint = AddOffset(startPoint, offsetStart);
            while (ZeroPixel(nextPoint) && offsetStart < 16)
            {
                offsetStart++;
                nextPoint = AddOffset(startPoint, offsetStart);
            }

            if (offsetStart >= 16)
            {
                throw new Exception("Could not find a next border pixel.");
            }

            return nextPoint;
        }

        

        // Pre-computes the border of the texture.
        // WARNING:  This algorithm assumes that the texture has non-zero pixels on its middle row.
        private List<Point> ComputeBorder()
        {
            int x = 0;
            int y = texture.Height/2;
            List<Point> b = new List<Point>();

            // Find the first non-zero pixel on the Height/2 row.
            while (x < texture.Width && ZeroPixel(x,y))
            {
                x++;
            }

            // If we have somehow gotten to the end of the texture, something is horribly wrong.
            if (x >= texture.Width)
            {
                throw new Exception("Texture has zero pixels on it's middle row.");
            }
            
            // We have found our first border pixel.  Start constructiong the border.
            b.Add(new Point(x, y));
            Point nextPoint;
            while ((nextPoint = ComputeBorderHelper(b)) != b.First())
            {
                b.Add(nextPoint);
            }

            return b;
        }
    }
}
