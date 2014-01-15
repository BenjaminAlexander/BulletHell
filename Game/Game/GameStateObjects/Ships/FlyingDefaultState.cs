using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.Ships
{
    class FlyingDefaultState : FlyingState
    {
        public FlyingDefaultState(FlyingStrategy context)
            : base(context)
        {
        }

        public FlyingDefaultState(FlyingStrategy context) : base(context)
        {

        }

        // Does nothing.
        public override void Handle(GameTime elapsedTime)
        {
        }
    }
}
