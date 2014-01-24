using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.GameStateObjects;

namespace MyGame.AI
{
    public class FlyingFleeState : FlyingState
    {
        public FlyingFleeState(FlyingStrategy context, FlyingGameObject obj, FlyingGameObject followObj)
            : base(context)
        {
            this.obj = obj;
            this.followObj = followObj;
        }

        public override void Handle(GameTime elapsedTime)
        {
            obj.TurnAway(elapsedTime, followObj.Position);
            obj.SpeedUp = true;
        }

        // TODO: correct the wonkey visibility.
        protected FlyingGameObject obj;
        protected FlyingGameObject followObj;
    }
}
