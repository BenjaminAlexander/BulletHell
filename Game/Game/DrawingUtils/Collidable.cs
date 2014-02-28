using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyGame.Utils;
using MyGame.Geometry;
namespace MyGame.DrawingUtils
{
    public class Collidable
    {
        LoadedTexture loadedTexture;

        public LoadedTexture LoadedTexture
        {
            get { return loadedTexture; }
        }

        public Color Color
        {
            set { color = value; }
            get { return color; }
        }

        public Vector2 Origin
        {
            get { return origin; }
        }

        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        private Color color;
        private Vector2 origin;
        private float depth;

        public Collidable(LoadedTexture t, Color c, Vector2 o, float d)
        {
            loadedTexture = t;
            color = c;
            origin = o;
            depth = d;
        }

        public Boolean CollidesWith(Vector2 position, float rotation, Collidable other, Vector2 otherPosition, float otherRotation)
        {
            //return Vector2.Distance(other.position, position) < 200;

            Rectangle tb = this.BoundingRectangle(position, rotation);
            Rectangle ob = other.BoundingRectangle(otherPosition, otherRotation);

            Circle thisCirlce = Circle.CreateBounding(tb);
            Circle otherCirlce = Circle.CreateBounding(ob);

            if (thisCirlce.Intersects(otherCirlce) && tb.Intersects(ob))
            {
                Color[] thisTextureData = loadedTexture.Data;
                Color[] otherTextureData = other.LoadedTexture.Data;
                if (MyIntersectPixels(this, other, position, rotation, otherPosition, otherRotation))
                {
                    return true;
                }
            }
            return false;
        }

        public Circle BoundingCircle(Vector2 position)
        {
            return new Circle(position, loadedTexture.BoundingCircle.Radius);
        }

        public void Draw(MyGraphicsClass graphics, Vector2 position, float rotation)
        {
            graphics.getSpriteBatch().Draw(loadedTexture.Texture, position, null, color, rotation, origin, 1, SpriteEffects.None, depth);
        }

        public Color[] GetColorData()
        {
            return loadedTexture.Data;
        }

        // Returns a matrix tranformation that sends a texture into it's actual position in the game world.
        public Matrix GetWorldTransformation(Vector2 position, float rotation)
        {
            Matrix originM = Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0);
            Matrix rotationM = Matrix.CreateRotationZ(rotation);
            //Matrix scaleM = Matrix.CreateScale(new Vector3(scale, scale, 1)); ;
            Matrix positionM = Matrix.CreateTranslation(position.X, position.Y, 0);

            //Matrix returnM = positionM * scaleM * rotationM * originM;
            Matrix returnM = originM * rotationM * /*scaleM * */ positionM;
            //Matrix returnM = scaleM * positionM;
            return returnM;

        }

        public Rectangle BoundingRectangle(Vector2 position, float rotation)
        {
            return CalculateBoundingRectangle(loadedTexture.BoundingRectangle, GetWorldTransformation(position, rotation));
        }

        // Caculates the world tranformed border of the collidable.
        public List<Point> Border(Vector2 position, float rotation)
        {
            List<Point> ret = new List<Point>();
            Matrix t = GetWorldTransformation(position, rotation);
            foreach (Point borderPoint in loadedTexture.Border)
            {
                Vector2 vec = Vector2Utils.PointToVector(borderPoint);
                ret.Add(Vector2Utils.VectorToPoint(Vector2.Transform(vec, t)));
            }
            return ret;
        }

        /// <summary>
        /// Microsoft XNA Community Game Platform
        /// Copyright (C) Microsoft Corporation. All rights reserved.
        /// 
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        //This one is better because it only checks the part the bounding rectangeles that intersect instead of the whole texture
        private static bool MyIntersectPixels(Collidable d1, Collidable d2, Vector2 position1, float rotation1, Vector2 position2, float rotation2)
        {
            // The World-coordinate intersection between the two bounding rectangles.
            Rectangle d1Bound = d1.BoundingRectangle(position1, rotation1);
            Rectangle d2Bound = d2.BoundingRectangle(position2, rotation2);
            Rectangle intersectArea = Rectangle.Intersect(d1Bound, d2Bound);

            // Pick the smaller border.  We will only check the smaller border points.
            List<Point> smallBorder;
            Matrix largeShapeInverseTransform;
            Collidable largeBorderCollidable;
            if (d1.LoadedTexture.Border.Count > d2.LoadedTexture.Border.Count)
            {
                smallBorder = d2.Border(position2, rotation2);
                largeShapeInverseTransform = Matrix.Invert(d1.GetWorldTransformation(position1, rotation1));
                largeBorderCollidable = d1;
            }
            else
            {
                smallBorder = d1.Border(position1, rotation1);
                largeShapeInverseTransform = Matrix.Invert(d2.GetWorldTransformation(position2, rotation2));
                largeBorderCollidable = d2;
            }

            foreach (Point borderPoint in smallBorder)
            {
                if (intersectArea.Contains(borderPoint))
                {
                    Point p = Vector2Utils.VectorToPoint(Vector2.Transform(Vector2Utils.PointToVector(borderPoint), largeShapeInverseTransform));

                    if (!largeBorderCollidable.LoadedTexture.ZeroPixel(p))
                    {
                        return true;
                    }
                }
            }

            ////randomly selecting a pixels to check instead of iterating through rows would improve performance
            //for (int worldX = intersectArea.X; worldX < intersectArea.X + intersectArea.Width; worldX++)
            //{
            //    for (int worldY = intersectArea.Y; worldY < intersectArea.Y + intersectArea.Height; worldY++)
            //    {
            //        Vector3 pos = new Vector3(worldX, worldY, 0);

            //        Vector3 texture1Pos = Vector3.Transform(pos, inversTransform1);
            //        Vector3 texture2Pos = Vector3.Transform(pos, inversTransform2);

            //        Point point1 = new Point((int)Math.Round(texture1Pos.X), (int)Math.Round(texture1Pos.Y));
            //        Point point2 = new Point((int)Math.Round(texture2Pos.X), (int)Math.Round(texture2Pos.Y));

            //        if (!d1.LoadedTexture.ZeroPixel(point1) && !d2.LoadedTexture.ZeroPixel(point2))
            //        {
            //            return true;
            //        }
            //    }
            //}

            return false;
        }

        public Boolean Contains(Vector2 point, Vector2 position, float rotation)
        {
            if (this.BoundingRectangle(position, rotation).Contains(new Point((int)(point.X), (int)(point.Y))))
            {
                Matrix inversTransform = Matrix.Invert(this.GetWorldTransformation(position, rotation));
                Color[] data = LoadedTexture.Data;

                Vector2 texturePos = Vector2.Transform(point, inversTransform);
                int x = (int)Math.Round(texturePos.X);
                int y = (int)Math.Round(texturePos.Y);

                if (0 <= x && x < LoadedTexture.Texture.Width &&
                        0 <= y && y < LoadedTexture.Texture.Height)
                {
                    Color color = data[x + y * LoadedTexture.Texture.Width];
                    if (color.A != 0)
                    {

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Microsoft XNA Community Game Platform
        /// Copyright (C) Microsoft Corporation. All rights reserved.
        /// 
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }
    }

}
