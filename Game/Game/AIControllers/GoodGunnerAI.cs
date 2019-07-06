using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.PlayerControllers;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.Engine.GameState.Instants;

namespace MyGame.AIControllers
{
    class GoodGunnerAI : AbstractAIController
    {
        private Ship target = null;
        private ServerGame game;

        public GoodGunnerAI(ServerGame game, Ship target)
            : base(game)
        {
            this.game = game;
        }

        public override void Update(CurrentInstant current, NextInstant next, float secondsElapsed)
        {
            //throw new NotImplementedException();
            Vector2 aim = this.Aimpoint;
            Boolean fire = this.Fire;

            if (target != null)
            {
                aim = target.Position[current] - (Vector2)this.Focus.Position[current];
                fire = Vector2.Distance(target.Position[current], this.Focus.Position[current]) < 3000;
            }

            this.Aimpoint = aim;
            this.Fire = fire;
        }
    }
}
