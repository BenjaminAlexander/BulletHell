using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.GameStateObjects;
namespace MyGame.PlayerControllers
{
    public class NetworkPlayerController: IController
    {
        private int id;
        private PlayerControlState state = new PlayerControlState(new Vector2(0), new Vector2(0), false);
        public Ship Focus
        {
            set { StaticControllerFocus.SetFocus(id, value); }
            get { return StaticControllerFocus.GetFocus(id); }
        }

        public NetworkPlayerController(int id)
        {
            this.id = id;
        }

        public PlayerControlState CurrentState
        {
            get
            {
                return state;
            }
        }

        public void Apply(PlayerControllerUpdate message)
        {
            state = new PlayerControlState(message);
        }



        public void Update(float secondsElapsed)
        {

        }
    }
}
