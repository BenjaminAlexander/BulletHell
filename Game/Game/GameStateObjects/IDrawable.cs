using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    public interface IDrawable
    {
        void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics);
    }
}
