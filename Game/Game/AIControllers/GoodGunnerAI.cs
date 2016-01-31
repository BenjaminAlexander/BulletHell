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
    class GoodGunnerAI : AbstractAIController
    {
        private Ship target = null;
        private ServerGame game;

        public GoodGunnerAI(ServerGame game)
            : base(game)
        {
            this.game = game;
        }

        public override void Update(float secondsElapsed)
        {
            //throw new NotImplementedException();
            Vector2 aim = this.Aimpoint;
            Boolean fire = this.Fire;

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
                aim = target.Position - this.Focus.Position;
                fire = Vector2.Distance(target.Position, this.Focus.Position) < 3000;
            }

            this.Aimpoint = aim;
            this.Fire = fire;
        }
    }
}
