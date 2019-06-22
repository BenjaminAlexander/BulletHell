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

namespace MyGame.AIControllers
{
    public class EvilAI : AbstractAIController
    {
        Vector2 flyTowardsRelative = new Vector2(0);

        private Ship target = null;
        private ServerGame game;

        public EvilAI(ServerGame game) : base(game)
        {
            this.game = game;
        }

        public override void Update(float secondsElapsed)
        {
            //throw new NotImplementedException();
            Vector2 aim = this.Aimpoint;
            float targetAngle = 0;
            float angleControl = 0;
            Boolean fire = this.Fire;

            if (target == null)
            {
                target = null;

                List<Ship> ships = new List<Ship>();
                foreach (Tower t in this.game.GameObjectCollection.GetMasterList().GetList<Tower>())
                {
                    ships.Add(t);
                }
                foreach (BigShip t in this.game.GameObjectCollection.GetMasterList().GetList<BigShip>())
                {
                    ships.Add(t);
                }

                ships.Remove(this.Focus);
                if (ships.Count > 0)
                {
                    target = ships[RandomUtils.random.Next(ships.Count)];
                }
            }
            else
            {
                if (Vector2.Distance(target.Position, this.Focus.Position) > 1000)
                {
                    flyTowardsRelative = new Vector2(0);
                }

                if (Vector2.Distance(target.Position, this.Focus.Position) < 500 && flyTowardsRelative == new Vector2(0))
                {
                    flyTowardsRelative = Utils.Vector2Utils.ConstructVectorFromPolar(2000, (float)(RandomUtils.random.NextDouble() * Math.PI * 2));
                }

                aim = target.Position - this.Focus.Position;
                targetAngle = Utils.Vector2Utils.RestrictAngle(Utils.Vector2Utils.Vector2Angle(target.Position - (this.Focus.Position + flyTowardsRelative)));
                angleControl = 1;
                fire = Vector2.Distance(target.Position, this.Focus.Position) < 3000;
            }

            this.Aimpoint = aim;
            this.AngleControl = angleControl;
            this.TargetAngle = targetAngle;
            this.MovementControl = 1;
            this.Fire = fire;
        }
    }
}
