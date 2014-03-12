using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;

namespace MyGame.PlayerControllers
{
    class AIController: IController
    {
        Vector2 flyTowardsRelative = new Vector2(0);
        Random rand = new Random();

        private Ship focus = null;
        private PlayerControlState state = new PlayerControlState(0, (float)(2 * Math.PI + 1), 0, new Vector2(0), true);

        public PlayerControlState CurrentState
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

            foreach(Ship s in StaticGameObjectCollection.Collection.GetMasterList().GetList<Ship>())
            {
                if(s != this.focus)
                {
                    if (Vector2.Distance(s.Position, this.focus.Position) > 1000)
                    {
                        flyTowardsRelative = new Vector2(0);
                    }

                    if (Vector2.Distance(s.Position, this.focus.Position) < 500 && flyTowardsRelative == new Vector2(0))
                    {
                        flyTowardsRelative = Utils.Vector2Utils.ConstructVectorFromPolar(2000, (float)(rand.NextDouble() * Math.PI * 2));
                    }

                    aim = s.Position - this.focus.Position;
                    targetAngle = Utils.Vector2Utils.RestrictAngle(Utils.Vector2Utils.Vector2Angle(s.Position - (this.focus.Position + flyTowardsRelative)));
                    angleControl = 1;
                    break;
                }
            }
            state = new PlayerControlState(angleControl, targetAngle, 1, aim, fire);
        }

        public GameStateObjects.Ship Focus
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
