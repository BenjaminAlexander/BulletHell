using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    public interface IUpdateable
    {
        void Update(float secondsElapsed);
    }
}
