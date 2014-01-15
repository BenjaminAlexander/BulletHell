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
            
        public abstract void Handle(GameTime elapsedTime);


        public virtual FlyingStrategy Context
        {
            get { return context; }
            private set { context = value; }
        }

        private FlyingStrategy context;
    }
}
