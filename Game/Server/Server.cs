using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using NetworkingLibrary;

namespace Server
{
    class GameServer
    {
        private TcpListener tcpListener;
        private Thread listenThread;

        public GameServer()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients()
        {

            Lobby lobby = new Lobby();
            lobby.StartLobby();

            this.tcpListener.Start();

            while (true)
            {
                Client clientobj = new Client(this.tcpListener.AcceptTcpClient());

                lobby.AddClient(clientobj);
            }
        }
    }
}
