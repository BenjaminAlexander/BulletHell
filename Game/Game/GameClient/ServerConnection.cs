using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using MyGame.Engine.Networking;
using System.Net;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameStateObjects;
using System.Threading;
using MyGame.PlayerControllers;
using Microsoft.Xna.Framework;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameClient
{
    //TODO: name this class better
    //maybe combine with local player
    public class ServerConnection : BasePlayer<GameObjectUpdate>
    {
        private LocalPlayerController controller;

        public ServerConnection(IPAddress serverAddress, ClientGame game)
            : base(new UdpTcpPair(serverAddress, 3000))
        {
            controller = new LocalPlayerController(game);
        }


        public override GameObjectUpdate GetUDPMessage(UdpTcpPair client)
        {
            return new GameObjectUpdate(client);
        }

        public void UpdateControlState(CurrentInstant current, GameTime gameTime)
        {
            controller.Update(current);
            this.SendUDP(controller.GetStateMessage(gameTime));
        }
    }
}
