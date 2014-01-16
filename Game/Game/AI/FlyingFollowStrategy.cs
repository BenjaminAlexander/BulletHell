using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;

namespace MyGame.AI
{
    class FlyingFollowStrategy : FlyingStrategy
    {
        public FlyingFollowStrategy(FlyingGameObject obj, FlyingGameObject followObj) : base(obj)
        {
            this.followObj = followObj;
            this.AddState(new FlyingFollowState(this, obj, followObj));
        }

        private FlyingGameObject followObj;
    }
}
