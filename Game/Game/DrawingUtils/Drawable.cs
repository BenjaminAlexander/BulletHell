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

namespace MyGame.DrawingUtils
{
    public class Drawable
    {
        private Texture2D texture;
        Vector2 position;
        Color color;
        float rotation;
        Vector2 origin;
        //float scale = 1;
        float depth;

        public Drawable(Texture2D texture, Vector2 position, Color color, float rotation, Vector2 origin, float depth)
        {
            this.texture = texture;
            this.position = position;
            this.color = color;
            this.rotation = rotation;
            this.origin = origin;
            //this.scale = scale;
            this.depth = depth;
        }

        public Vector2 Position
        {
            set { position = value; }
            get { return position; }
        }

        public float Rotation
        {
            set { rotation = value; }
            get { return rotation; }
        }

        public Color Color
        {
            set { color = value; }
            get { return color; }
        }

        public void Draw(MyGraphicsClass graphics)
        {
            graphics.getSpriteBatch().Draw(texture, position, null, color, rotation, origin, 1, SpriteEffects.None, depth);
            /*Rectangle bound = this.BoundingRectangle();
            graphics.DrawRectangle(new Vector2(bound.X, bound.Y), new Vector2(bound.Width, bound.Height), new Vector2(0), 0, Color.Black, 1);*/
            

        }

        private Color[] GetColorData()
        {
            Color[] thisTextureData = new Color[this.texture.Width * this.texture.Height];
            this.texture.GetData(thisTextureData);
            return thisTextureData;
        }

        public Boolean CollidesWith(Drawable other)
        {
            //return Vector2.Distance(other.position, position) < 200;

            Rectangle tb = this.BoundingRectangle();
            Rectangle ob = other.BoundingRectangle();
            
            if (tb.Intersects(ob))
            {

                Color[] thisTextureData = new Color[this.texture.Width * this.texture.Height];
                this.texture.GetData(thisTextureData);

                Color[] otherTextureData = new Color[other.texture.Width * other.texture.Height];
                other.texture.GetData(otherTextureData);

                if (IntersectPixels(this.GetWorldTransformation(), this.texture.Width,
                                        this.texture.Height, thisTextureData,
                                        other.GetWorldTransformation(), other.texture.Width,
                                        other.texture.Height, otherTextureData))
                //if(MyIntersectPixels(this, other))
                {
                    return true;
                }
            }
            return false;
        }

        private static int ClosestIntInLimit(int initial, int max)
        {
            if (initial < 0)
            {
                return 0;
            }
            if (initial >= max)
            {
                return max - 1;
            }
            return initial;
        }

        private static bool MyIntersectPixels(Drawable d1, Drawable d2)
        {
            Rectangle d1Bound = d1.BoundingRectangle();
            Rectangle d2Bound = d2.BoundingRectangle();

            Rectangle intersectArea;
            Rectangle.Intersect(ref d1Bound, ref d2Bound, out intersectArea);

            Matrix inversTransform1 = Matrix.Invert(d1.GetWorldTransformation());
            Matrix inversTransform2 = Matrix.Invert(d2.GetWorldTransformation());

            Color[] data1 = d1.GetColorData();
            Color[] data2 = d2.GetColorData();

            //randomly selecting a pixels to check instead of iterating through rows would improve performance
            for (int worldX = intersectArea.X; worldX < intersectArea.X + intersectArea.Width; worldX++)
            {
                for (int worldY = intersectArea.Y; worldY < intersectArea.Y + intersectArea.Height; worldY++)
                {
                    Vector3 pos = new Vector3(worldX, worldY, 0);

                    Vector3 texture1Pos = Vector3.Transform(pos, inversTransform1);
                    Vector3 texture2Pos = Vector3.Transform(pos, inversTransform2);

                    int x1 = ClosestIntInLimit((int)Math.Round(texture1Pos.X), d1.texture.Width);
                    int y1 = ClosestIntInLimit((int)Math.Round(texture1Pos.Y), d1.texture.Height);

                    int x2 = ClosestIntInLimit((int)Math.Round(texture2Pos.X), d2.texture.Width);
                    int y2 = ClosestIntInLimit((int)Math.Round(texture2Pos.Y), d2.texture.Height);

                    
                    if (0 <= x1 && x1 < d1.texture.Width &&
                        0 <= y1 && y1 < d1.texture.Height &&
                        0 <= x2 && x2 < d2.texture.Width &&
                        0 <= y2 && y2 < d2.texture.Height)
                    {
                        Color color1 = data1[x1 + y1 * d1.texture.Width];
                        Color color2 = data2[x2 + y2 * d2.texture.Width];

                        // If both pixels are not completely transparent,
                        if (color1.A != 0 && color2.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private Matrix GetWorldTransformation()
        {
            Matrix originM = Matrix.CreateTranslation(-origin.X, -origin.Y, 0);
            Matrix rotationM = Matrix.CreateRotationZ(rotation);
            //Matrix scaleM = Matrix.CreateScale(new Vector3(scale, scale, 1)); ;
            Matrix positionM = Matrix.CreateTranslation(position.X, position.Y, 0);
            
            //Matrix returnM = positionM * scaleM * rotationM * originM;
            Matrix returnM =  originM * rotationM * /*scaleM * */ positionM;
            //Matrix returnM = scaleM * positionM;
            return returnM;

        }

        private Rectangle BoundingRectangle()
        {
            return CalculateBoundingRectangle(TextureBoundingRectangle(), GetWorldTransformation());
        }

        private Rectangle TextureBoundingRectangle()
        {
            return new Rectangle(0, 0, texture.Width, texture.Height);
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
