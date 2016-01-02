﻿using System;
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
            //TODO: what is this and why is it here?
            GameMessage.Initialize();

            Lobby lobby = new Lobby();
            lobby.Run();
        }
    }
}