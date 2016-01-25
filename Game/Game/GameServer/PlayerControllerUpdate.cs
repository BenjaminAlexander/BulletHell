using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameServer
{
    //TODO: make this class useful again after the lobby/networkplayermanager mashup
    public class PlayerControllerUpdate : GameMessage
    {
        public PlayerControllerUpdate(GameTime currentGameTime) : base(currentGameTime)
        {

        }

        public PlayerControllerUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
        }

        internal void Apply(LobbyClient client)
        {
            client.controller.ApplyUpdate(this);
        }
    }
}
