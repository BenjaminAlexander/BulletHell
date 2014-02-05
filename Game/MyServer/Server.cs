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
        private TcpListener prelimListener;
        private Thread listenThread;
        private static int nextClientID = 1;
        public GameServer()
        {
            TCPMessage.Initialize();
            this.prelimListener = new TcpListener(IPAddress.Any, 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients()
        {

            Lobby lobby = new Lobby();
            lobby.StartLobby();

            this.prelimListener.Start();

            while (true)
            {
                TcpClient prelimClient = this.prelimListener.AcceptTcpClient();
                (new ClientID(nextClientID)).SendTCP(prelimClient.GetStream(), new Mutex());
                prelimClient.Close();

                TcpListener tcpListener = new TcpListener(IPAddress.Any, nextClientID + 3000);
                tcpListener.Start();
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpListener.Stop();


                UdpClient udpClient = new UdpClient((IPEndPoint)tcpClient.Client.LocalEndPoint);
                udpClient.Connect((IPEndPoint)tcpClient.Client.RemoteEndPoint);
                Client clientobj = new Client(tcpClient, udpClient, nextClientID);

                lobby.AddClient(clientobj);
                nextClientID++;
            }
        }
    }
}
