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
                    aim = s.Position - focus.Position;
                    targetAngle = Utils.Vector2Utils.RestrictAngle(Utils.Vector2Utils.Vector2Angle(aim));
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
