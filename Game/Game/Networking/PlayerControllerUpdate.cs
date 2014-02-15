using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;

namespace MyGame.Networking
{
    public class PlayerControllerUpdate : GameUpdate
    {
        public PlayerControllerUpdate()
        {

        }

        public PlayerControllerUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
        }

        public override void Apply(Game1 game)
        {
            if (Game1.IsServer)
            {
                StaticNetworkPlayerManager.Apply(this);
            }
        }
    }
}
