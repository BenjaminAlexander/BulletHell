using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MyGame.DrawingUtils
{
    public static class Textures
    {
        private static Texture2D ship;
        public static Texture2D Ship
        {
            get { return ship; }
        }

        private static Texture2D bullet;
        public static Texture2D Bullet
        {
            get { return bullet; }
        }

        public static void LoadContent(ContentManager content)
        {
            ship = content.Load<Texture2D>("Ship");
            bullet = content.Load<Texture2D>("Bullet");
        }
    }
}
