﻿using System;
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
            Matrix positionM = Matrix.CreateTranslation(position.X, position.Y, 0);

            Matrix returnM = originM * rotationM * positionM;
            return returnM;

        }

        public Rectangle BoundingRectangle(Vector2 position, float rotation)
        {
            return CalculateBoundingRectangle(loadedTexture.BoundingRectangle, GetWorldTransformation(position, rotation));
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
            Rectangle d1Bound = d1.BoundingRectangle(position1, rotation1);
            Rectangle d2Bound = d2.BoundingRectangle(position2, rotation2);

            Rectangle intersectArea;
            Rectangle.Intersect(ref d1Bound, ref d2Bound, out intersectArea);

            Matrix inversTransform1 = Matrix.Invert(d1.GetWorldTransformation(position1, rotation1));
            Matrix inversTransform2 = Matrix.Invert(d2.GetWorldTransformation(position2, rotation2));

            Color[] data1 = d1.LoadedTexture.Data;
            Color[] data2 = d2.LoadedTexture.Data;

            //randomly selecting a pixels to check instead of iterating through rows would improve performance
            for (int worldX = intersectArea.X; worldX < intersectArea.X + intersectArea.Width; worldX++)
            {
                for (int worldY = intersectArea.Y; worldY < intersectArea.Y + intersectArea.Height; worldY++)
                {
                    Vector3 pos = new Vector3(worldX, worldY, 0);

                    Vector3 texture1Pos = Vector3.Transform(pos, inversTransform1);
                    Vector3 texture2Pos = Vector3.Transform(pos, inversTransform2);

                    int x1 = (int)Math.Round(texture1Pos.X);
                    int y1 = (int)Math.Round(texture1Pos.Y);

                    int x2 = (int)Math.Round(texture2Pos.X);
                    int y2 = (int)Math.Round(texture2Pos.Y);


                    if (0 <= x1 && x1 < d1.LoadedTexture.Texture.Width &&
                        0 <= y1 && y1 < d1.LoadedTexture.Texture.Height &&
                        0 <= x2 && x2 < d2.LoadedTexture.Texture.Width &&
                        0 <= y2 && y2 < d2.LoadedTexture.Texture.Height)
                    {
                        Color color1 = data1[x1 + y1 * d1.LoadedTexture.Texture.Width];
                        Color color2 = data2[x2 + y2 * d2.LoadedTexture.Texture.Width];

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
                    if (color.A != 0 )
                    {

                        return true;
                    }
                }
            }
            return false;
        }
    }
}
