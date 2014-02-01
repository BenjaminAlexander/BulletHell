using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using MyGame.Networking;

namespace MyServer
{
    class GameServer
    {
        private TcpListener tcpListener;
        private Thread listenThread;

        public GameServer()
        {
            TCPMessage.Initialize();
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
                TcpClient tcpClient = this.tcpListener.AcceptTcpClient();
                UdpClient udpClient = new UdpClient((IPEndPoint)tcpClient.Client.LocalEndPoint);
                udpClient.Connect((IPEndPoint)tcpClient.Client.RemoteEndPoint);
                Client clientobj = new Client(tcpClient, udpClient);

                lobby.AddClient(clientobj);
            }
        }
    }
}
