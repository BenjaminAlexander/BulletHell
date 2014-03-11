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
        private PlayerControlState state = new PlayerControlState(new Vector2(500,50), new Vector2(0), true);

        public PlayerControlState CurrentState
        {
            get { return state; }
        }

        public void Update(float secondsElapsed)
        {
            //throw new NotImplementedException();
            Vector2 aim = state.Aimpoint;
            Vector2 move = state.Move;
            Boolean fire = state.Fire;

            foreach(Ship s in StaticGameObjectCollection.Collection.GetMasterList().GetList<Ship>())
            {
                if(s != this.focus)
                {
                    aim = s.Position - focus.Position;
                    move = Utils.Vector2Utils.RotateVector2(s.Position - focus.Position, -focus.Direction);
                    move = new Vector2(move.Y, -move.X);
                    break;
                }
            }
            state = new PlayerControlState(aim, move, fire);
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
