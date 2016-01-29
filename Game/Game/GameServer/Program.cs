using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using MyGame.Networking;

namespace MyGame.GameServer
{
    public static class Program
    {
        public static void ServerMain()
        {
            Lobby lobby = new Lobby();
            lobby.Run();
        }
    }
}
