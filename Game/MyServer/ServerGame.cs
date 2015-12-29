using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;

namespace MyServer
{
    class ServerGame : Game1
    {
        //TODO: there needs to be a better way to set up game-mode-ish parameters
        private static Vector2 SetWorldSize(Lobby lobby)
        {
            Vector2 worldSize = new Vector2(20000);
            lobby.BroadcastTCP(new SetWorldSize(worldSize));
            return worldSize;
        }

        public ServerGame(Lobby lobby, ThreadSafeQueue<GameMessage> outgoingQueue, ThreadSafeQueue<GameMessage> incomingQueue)
            : base(outgoingQueue, incomingQueue, 0/*TODO: 0 is the player ID of the server, this should probably change*/, SetWorldSize(lobby))
        {
            foreach (Client client in lobby.Clients)
            {
                StaticNetworkPlayerManager.Add(client.GetID());
            }
        }
    }
}
