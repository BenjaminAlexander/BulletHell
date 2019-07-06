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
    public class EvilAI : AbstractAIController
    {
        Vector2 flyTowardsRelative = new Vector2(0);

        private Ship target = null;
        private ServerGame game;

        public EvilAI(ServerGame game, Ship target) : base(game)
        {
            this.game = game;
            this.target = target;
        }

        public override void Update(CurrentInstant current, NextInstant next, float secondsElapsed)
        {
            //throw new NotImplementedException();
            Vector2 aim = this.Aimpoint;
            float targetAngle = 0;
            float angleControl = 0;
            Boolean fire = this.Fire;

            if (target != null)
            {
                if (Vector2.Distance(target.Position[current], this.Focus.Position[current]) > 1000)
                {
                    flyTowardsRelative = new Vector2(0);
                }

                if (Vector2.Distance(target.Position[current], this.Focus.Position[current]) < 500 && flyTowardsRelative == new Vector2(0))
                {
                    flyTowardsRelative = Utils.Vector2Utils.ConstructVectorFromPolar(2000, (float)(RandomUtils.random.NextDouble() * Math.PI * 2));
                }

                aim = (Vector2)target.Position[current] - (Vector2)this.Focus.Position[current];
                targetAngle = Utils.Vector2Utils.RestrictAngle(Utils.Vector2Utils.Vector2Angle(target.Position[current] - ((Vector2)this.Focus.Position[current] + flyTowardsRelative)));
                angleControl = 1;
                fire = Vector2.Distance(target.Position[current], this.Focus.Position[current]) < 3000;
            }

            this.Aimpoint = aim;
            this.AngleControl = angleControl;
            this.TargetAngle = targetAngle;
            this.MovementControl = 1;
            this.Fire = fire;
        }
    }
}
