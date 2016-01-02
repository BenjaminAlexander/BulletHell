using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

namespace MyGame.PlayerControllers
{
    public class NetworkPlayerController: IController
    {
        private int id;
        private Game1 game;
        private ControlState state = new ControlState(0, (float)(2 * Math.PI + 1), 0, new Vector2(0), false);
        public Ship Focus
        {
            set { StaticControllerFocus.SetFocus(id, value, this.game.GameObjectCollection); }
            get { return StaticControllerFocus.GetFocus(id); }
        }

        public NetworkPlayerController(int id, Game1 game)
        {
            this.id = id;
            this.game = game;
        }

        public ControlState CurrentState
        {
            get
            {
                return state;
            }
        }

        public void Apply(PlayerControllerUpdate message)
        {
            state = new ControlState(message);
        }



        public void Update(float secondsElapsed)
        {

        }
    }
}
