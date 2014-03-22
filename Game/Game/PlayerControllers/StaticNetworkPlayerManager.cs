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

        public static void Add(int playerID)
        {
            playerStatesMutex.WaitOne();
            playerStates.Add(playerID, new NetworkPlayerController(playerID));
            playerStatesMutex.ReleaseMutex();
        }

        public static void Apply(PlayerControllerUpdate message)
        {
            //TODO: how do we stop cheating here?  
            //Clients can just change the id they send 
            //to the server to inpersonate other players
            Game1.AsserIsServer();
            playerStatesMutex.WaitOne();
            playerStates[message.ClientID].Apply(message);
            playerStatesMutex.ReleaseMutex();
        }

        public static NetworkPlayerController GetController(int i)
        {
            Game1.AsserIsServer();
            if (!playerStates.ContainsKey(i))
            {
                Add(i);
            }

            playerStatesMutex.WaitOne();
            NetworkPlayerController rtn = playerStates[i];
            playerStatesMutex.ReleaseMutex();
            return rtn;
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
