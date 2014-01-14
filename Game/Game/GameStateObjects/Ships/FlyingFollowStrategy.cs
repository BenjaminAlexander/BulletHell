using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.Ships
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
