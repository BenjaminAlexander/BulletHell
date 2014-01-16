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

namespace MyGame.DrawingUtils
{
    public class Drawable
    {
        private Texture2D texture;
        private Vector2 position;
        private Color color;
        private float rotation;
        private Vector2 origin;
        private float depth;

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

        public void Draw(MyGraphicsClass graphics)
        {
            graphics.getSpriteBatch().Draw(texture, position, null, color, rotation, origin, 1, SpriteEffects.None, depth);
            /*Rectangle bound = this.BoundingRectangle();
            graphics.DrawRectangle(new Vector2(bound.X, bound.Y), new Vector2(bound.Width, bound.Height), new Vector2(0), 0, Color.Black, 1);*/
        }

        protected Color[] GetColorData()
        {
            Color[] thisTextureData = new Color[this.texture.Width * this.texture.Height];
            this.texture.GetData(thisTextureData);
            return thisTextureData;
        }

        protected Matrix GetWorldTransformation()
        {
            Matrix originM = Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0);
            Matrix rotationM = Matrix.CreateRotationZ(Rotation);
            //Matrix scaleM = Matrix.CreateScale(new Vector3(scale, scale, 1)); ;
            Matrix positionM = Matrix.CreateTranslation(Position.X, Position.Y, 0);

            //Matrix returnM = positionM * scaleM * rotationM * originM;
            Matrix returnM = originM * rotationM * /*scaleM * */ positionM;
            //Matrix returnM = scaleM * positionM;
            return returnM;

        }

        protected Rectangle BoundingRectangle()
        {
            return CalculateBoundingRectangle(TextureBoundingRectangle(), GetWorldTransformation());
        }

        private Rectangle TextureBoundingRectangle()
        {
            return new Rectangle(0, 0, Texture.Width, Texture.Height);
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

        public Texture2D Texture
        {
            get { return texture; }
            protected set { texture = value; }
        }

        public Vector2 Position
        {
            set { position = value; }
            get { return position; }
        }

        public Color Color
        {
            set { color = value; }
            get { return color; }
        }

        public float Rotation
        {
            set { rotation = value; }
            get { return rotation; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            protected set { origin = value; }
        }

        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }
        
    }
}
