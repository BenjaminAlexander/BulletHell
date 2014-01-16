using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;

namespace MyGame.AI
{
    class FlyingFollowState : FlyingState
    {
        public FlyingFollowState(FlyingStrategy context, FlyingGameObject obj, FlyingGameObject followObj) : base(context)
        {
            this.obj = obj;
            this.followObj = followObj;
        }

        public override void Handle(GameTime elapsedTime)
        {
            obj.TurnTowards(elapsedTime, followObj.Position);
            obj.Acceleration = 100;
        }

        // TODO: correct the wonkey visibility.
        protected FlyingGameObject obj;
        protected FlyingGameObject followObj;
    }
}
