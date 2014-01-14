using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.Ships
{
    abstract class FlyingState
    {
        public FlyingState(FlyingStrategy context)
        {
            this.context = context;
        }
            
        public void Handle(GameTime elapsedTime);

        private FlyingStrategy context;
    }
}
