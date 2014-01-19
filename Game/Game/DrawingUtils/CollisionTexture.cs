using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        public CollisionTexture(ContentManager content, String contentName)
        {
            texture = content.Load<Texture2D>(contentName);
            data = new Color[this.texture.Width * this.texture.Height];
            this.texture.GetData(data);
        }
    }
}
