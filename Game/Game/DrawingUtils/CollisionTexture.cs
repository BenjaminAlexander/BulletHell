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

        private Vector2 centerOfMass;
        public Vector2 CenterOfMass
        {
            get { return centerOfMass; }
        }

        public CollisionTexture(ContentManager content, String contentName)
        {
            texture = content.Load<Texture2D>(contentName);
            data = new Color[this.texture.Width * this.texture.Height];
            this.texture.GetData(data);

            Vector2 sum = new Vector2(0);
            int count = 0;
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    Color color = data[x + y * texture.Width];
                    if (color.A != 0)
                    {
                        sum = sum + new Vector2(x, y);
                        count++;
                    }
                }
            }
            centerOfMass = sum / count;
        }
    }
}
