using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using Microsoft.Xna.Framework;
using MyGame.GameServer;

namespace MyGame.Networking
{
    public class PlayerControllerUpdate : ServerUpdate
    {
        public PlayerControllerUpdate(GameTime currentGameTime) : base(currentGameTime)
        {

        }

        public PlayerControllerUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
        }

        public override void Apply(ServerGame game)
        {
            game.NetworkPlayerManager.Apply(this);
        }
    }
}
