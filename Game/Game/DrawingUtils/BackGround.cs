using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.DrawingUtils
{
    class BackGround
    {
        private List<Vector2> stars = new List<Vector2>();
        private Collidable star = new Collidable(TextureLoader.GetTexture("Star"), Color.SteelBlue, new Vector2(25), .1f);
        private Random random = new Random();

        public BackGround(Vector2 worldSize)
        {
            for (int i = 0; i < (int)(worldSize.X * worldSize.Y / 50000); i++)
            {
                stars.Add(Utils.MathUtils.RandomVector(worldSize));
            }
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime, MyGraphicsClass graphics)
        {
            foreach (Vector2 obj in stars)
            {
                star.Draw(graphics, obj, 0);
            }
        }
    }
}
