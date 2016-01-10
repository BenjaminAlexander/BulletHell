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
        //TODO:  this sort of looks similar to the lobby

        private Dictionary<int, NetworkPlayerController> playerStates = new Dictionary<int, NetworkPlayerController>();
        private Mutex playerStatesMutex = new Mutex(false);

        // Adds a network controller for clientID.  This is usually called by lobby when a new client is added to the lobby.
        public void Add(int clientID)
        {
            playerStatesMutex.WaitOne();
            playerStates.Add(clientID, new NetworkPlayerController(clientID));
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

        public List<int> ControllersIDs
        {
            get 
            {
                playerStatesMutex.WaitOne();
                List<int> rtn = playerStates.Keys.ToList();
                playerStatesMutex.ReleaseMutex();
                return rtn; 
            }
        }

        public NetworkPlayerController this[int id]
        {
            get
            {
                playerStatesMutex.WaitOne();
                NetworkPlayerController rtn = playerStates[id];
                playerStatesMutex.ReleaseMutex();
                return rtn;
            }
        }
    }
}
