using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.PlayerControllers
{
    static class StaticNetworkPlayerManager
    {
        private static int playerCount = 4;
        private static Dictionary<int, NetworkPlayerController> playerStates = new Dictionary<int, NetworkPlayerController>();
        private static Boolean initialized = false;

        private static void Initialize()
        {
            //TODO: how do we know which client IDs are valid?
            for (int i = 1; i < playerCount+1; i++)
            {
                playerStates.Add(i, new NetworkPlayerController());
            }
            initialized = true;
        }

        public static void Apply(PlayerControllerUpdate message)
        {
            Game1.AsserIsServer();
            if (!initialized)
            {
                Initialize();
            }
            playerStates[message.ClientID].Apply(message);
        }

        public static NetworkPlayerController GetController(int i)
        {
            Game1.AsserIsServer();
            if (!initialized)
            {
                Initialize();
            }
            return playerStates[i];
        }
    }
}
