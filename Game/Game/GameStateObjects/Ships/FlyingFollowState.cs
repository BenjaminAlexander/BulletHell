using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.Ships
{
    class FlyingFollowState : FlyingState
    {
        public FlyingFollowState(FlyingFollowStrategy context, FlyingGameObject obj, FlyingGameObject followObj) : base(context)
        {
            this.obj = obj;
            this.followObj = followObj;
        }

        public override void Handle(GameTime elapsedTime)
        {
            obj.TurnTowards(elapsedTime, followObj.Position);
            obj.Acceleration = 100;
        }

        FlyingGameObject obj;
        FlyingGameObject followObj;
    }
}
