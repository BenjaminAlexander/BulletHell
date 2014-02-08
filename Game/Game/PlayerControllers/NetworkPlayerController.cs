using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.PlayerControllers
{
    class NetworkPlayerController
    {
        private PlayerControlState state = new PlayerControlState(new Vector2(0), new Vector2(0), false);
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

    }
}
