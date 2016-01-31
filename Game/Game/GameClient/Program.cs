using System;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.VisualBasic;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameStateObjects;

namespace MyGame.GameClient
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void ClientMain()
        {
            string serverIP = Microsoft.VisualBasic.Interaction.InputBox("Enter Server IP Address", "Server IP Address", "127.0.0.1");

            IPAddress address;
            try
            {
                address = IPAddress.Parse(serverIP);
            }
            catch (System.FormatException)
            {
                return;
            }

            ClientGame game = new ClientGame(address);
            game.Run();
        }
    }
}
