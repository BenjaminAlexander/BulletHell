using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

namespace MyGame.PlayerControllers
{
    class AIController: IController
    {
        Vector2 flyTowardsRelative = new Vector2(0);
        static Random rand = new Random();

        private Ship focus = null;
        private ControlState state = new ControlState(0, (float)(2 * Math.PI + 1), 0, new Vector2(0), true);

        private Ship target = null;

        public ControlState CurrentState
        {
            get { return state; }
        }

        public void Update(float secondsElapsed)
        {
            //throw new NotImplementedException();
            Vector2 aim = state.Aimpoint;
            float targetAngle = 0;
            float angleControl = 0;
            Boolean fire = state.Fire;

            if (target == null || target.IsDestroyed)
            {
                target = null;



                List<Ship> ships = new List<Ship>();
                foreach (Tower t in StaticGameObjectCollection.Collection.GetMasterList().GetList<Tower>())
                {
                    ships.Add(t);
                }
                foreach (BigShip t in StaticGameObjectCollection.Collection.GetMasterList().GetList<BigShip>())
                {
                    ships.Add(t);
                }

                ships.Remove(this.focus);
                if (ships.Count > 0)
                {
                    target = ships[rand.Next(ships.Count)];
                }
            }
            else
            {
                if (Vector2.Distance(target.Position, this.focus.Position) > 1000)
                {
                    flyTowardsRelative = new Vector2(0);
                }

                if (Vector2.Distance(target.Position, this.focus.Position) < 500 && flyTowardsRelative == new Vector2(0))
                {
                    flyTowardsRelative = Utils.Vector2Utils.ConstructVectorFromPolar(2000, (float)(rand.NextDouble() * Math.PI * 2));
                }

                aim = target.Position - this.focus.Position;
                targetAngle = Utils.Vector2Utils.RestrictAngle(Utils.Vector2Utils.Vector2Angle(target.Position - (this.focus.Position + flyTowardsRelative)));
                angleControl = 1;
                fire = Vector2.Distance(target.Position, this.focus.Position) < 3000;
            }

            state = new ControlState(angleControl, targetAngle, 1, aim, fire);
        }

        public Ship Focus
        {
            get
            {
                return focus;
            }
            set
            {
                focus = value;
            }
        }
    }
}
