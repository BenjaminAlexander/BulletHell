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
    class GoodAI : AbstractAIController
    {
        Vector2 targetPosition = new Vector2(0);

        private Ship target = null;
        private ServerGame game;

        public GoodAI(ServerGame game, Ship target)
            : base(game)
        {
            this.game = game;
            this.target = target;
        }

        public Vector2 TargetPosition
        {
            set
            {
                targetPosition = value;
                this.MovementControl = 1;
                this.AngleControl = 1;
            }
        }

        public override void Update(CurrentInstant current, NextInstant next, float secondsElapsed)
        {
            this.Fire = false;

            if (target != null)
            {
                this.Aimpoint = target.Position[current] - (Vector2)this.Focus.Position[current];
                this.Fire = Vector2.Distance(target.Position[current], this.Focus.Position[current]) < 3000;
            }

            float newDistance = Vector2.Distance(targetPosition, this.Focus.Position[current]);

            this.TargetAngle = Utils.Vector2Utils.RestrictAngle(Utils.Vector2Utils.Vector2Angle(targetPosition - this.Focus.Position[current]));
            
            if (newDistance < 300)
            {
                this.MovementControl = 0;
                this.AngleControl = 0;
            }
        }
    }
}
