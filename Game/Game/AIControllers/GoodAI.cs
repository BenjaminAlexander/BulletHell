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
    class GoodAI : AbstractAIController
    {
        Vector2 targetPosition = new Vector2(0);

        private Ship target = null;
        private ServerGame game;

        public GoodAI(ServerGame game)
            : base(game)
        {
            this.game = game;
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

        public override void Update(float secondsElapsed)
        {
            this.Fire = false;

            if (target == null || target.IsDestroyed)
            {
                target = null;

                List<Ship> ships = new List<Ship>();
                foreach (SmallShip t in this.game.GameObjectCollection.GetMasterList().GetList<SmallShip>())
                {
                    if (Vector2.Distance(t.Position, this.Focus.Position) < 3000)
                    {
                        ships.Add(t);
                    }
                }

                ships.Remove(this.Focus);
                if (ships.Count > 0)
                {
                    target = ships[RandomUtils.random.Next(ships.Count)];
                }
            }
            else
            {
                this.Aimpoint = target.Position - this.Focus.Position;
                this.Fire = Vector2.Distance(target.Position, this.Focus.Position) < 3000;
            }

            float newDistance = Vector2.Distance(targetPosition, this.Focus.Position);

            this.TargetAngle = Utils.Vector2Utils.RestrictAngle(Utils.Vector2Utils.Vector2Angle(targetPosition - this.Focus.Position));
            
            if (newDistance < 300)
            {
                this.MovementControl = 0;
                this.AngleControl = 0;
            }
        }
    }
}
