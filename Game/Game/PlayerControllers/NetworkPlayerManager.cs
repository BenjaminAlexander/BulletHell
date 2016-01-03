using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;
using System.Threading;
using MyGame.GameServer;

namespace MyGame.PlayerControllers
{
    public class NetworkPlayerManager
    {
        private Dictionary<int, NetworkPlayerController> playerStates = new Dictionary<int, NetworkPlayerController>();
        private Mutex playerStatesMutex = new Mutex(false);

        // Adds a network controller for clientID.  This is usually called by lobby when a new client is added to the lobby.
        public void Add(int clientID, ServerGame game)
        {
            playerStatesMutex.WaitOne();
            playerStates.Add(clientID, new NetworkPlayerController(clientID, game));
            playerStatesMutex.ReleaseMutex();
        }

        public void Apply(PlayerControllerUpdate message)
        {
            //TODO: how do we stop cheating here?  
            //Clients can just change the id they send 
            //to the server to inpersonate other players
            playerStatesMutex.WaitOne();
            playerStates[message.ClientID].ApplyUpdate(message);
            playerStatesMutex.ReleaseMutex();
        }

        public List<NetworkPlayerController> NetworkPlayerControllerList()
        {
            playerStatesMutex.WaitOne();
            List<NetworkPlayerController> rtn = playerStates.Values.ToList();
            playerStatesMutex.ReleaseMutex();
            return rtn;
        }
    }
}
