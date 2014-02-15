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

        private static Boolean initialized = false;

        private static void Initialize()
        {
            
            /*for (int i = 1; i < playerCount+1; i++)
            {
                Add(i);
            }*/
            initialized = true;
        }

        public static void Add(int playerID)
        {
            playerStatesMutex.WaitOne();
            playerStates.Add(playerID, new NetworkPlayerController(playerID));
            playerStatesMutex.ReleaseMutex();
        }

        public static void Apply(PlayerControllerUpdate message)
        {
            //TODO: how do we stop cheating here?
            Game1.AsserIsServer();
            if (!initialized)
            {
                Initialize();
            }
            playerStatesMutex.WaitOne();
            playerStates[message.ClientID].Apply(message);
            playerStatesMutex.ReleaseMutex();
        }

        public static NetworkPlayerController GetController(int i)
        {
            Game1.AsserIsServer();
            if (!initialized)
            {
                Initialize();
            }
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
