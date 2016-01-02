using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;
using System.Threading;

namespace MyGame.PlayerControllers
{
    public class StaticNetworkPlayerManager
    {
        private static Dictionary<int, NetworkPlayerController> playerStates = new Dictionary<int, NetworkPlayerController>();
        private static Mutex playerStatesMutex = new Mutex(false);

        // Adds a network controller for clientID.  This is usually called by lobby when a new client is added to the lobby.
        public static void Add(int clientID)
        {
            playerStatesMutex.WaitOne();
            playerStates.Add(clientID, new NetworkPlayerController(clientID));
            playerStatesMutex.ReleaseMutex();
        }

        public static void Apply(PlayerControllerUpdate message)
        {
            //TODO: how do we stop cheating here?  
            //Clients can just change the id they send 
            //to the server to inpersonate other players
            playerStatesMutex.WaitOne();
            playerStates[message.ClientID].Apply(message);
            playerStatesMutex.ReleaseMutex();
        }

        public static List<NetworkPlayerController> NetworkPlayerControllerList()
        {
            playerStatesMutex.WaitOne();
            List<NetworkPlayerController> rtn = playerStates.Values.ToList();
            playerStatesMutex.ReleaseMutex();
            return rtn;
        }
    }
}
