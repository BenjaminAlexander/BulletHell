using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.DrawingUtils
{
    class BackGround : GameStateObjects.IDrawable
    {
        private List<Drawable> stars = new List<Drawable>();
        private Random random = new Random();

        public BackGround(Vector2 worldSize)
        {
            for (int i = 0; i < (int)(worldSize.X * worldSize.Y / 50000); i++)
            {
                stars.Add(new Drawable(Textures.Star, Utils.MathUtils.RandomVector(worldSize), Color.SteelBlue, (float)(random.NextDouble() * Math.PI * 2), new Vector2(25), .1f));
            }
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime, MyGraphicsClass graphics)
        {
            foreach (Drawable obj in stars)
            {
                obj.Draw(graphics);
            }
        }
    }
}
