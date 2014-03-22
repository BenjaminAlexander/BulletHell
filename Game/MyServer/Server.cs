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
            GameMessage.Initialize();
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
                //Listen, connect, and then send the new client its ID, then disconnect
                TcpClient prelimClient = this.prelimListener.AcceptTcpClient();
                (new ClientID(nextClientID)).SendTCP(prelimClient.GetStream(), new Mutex());
                prelimClient.Close();

                //Start listening for that client on its new port
                TcpListener tcpListener = new TcpListener(IPAddress.Any, nextClientID + 3000);
                tcpListener.Start();
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpListener.Stop();

                //set up UDP and TCP connections
                UdpClient udpClient = new UdpClient((IPEndPoint)tcpClient.Client.LocalEndPoint);
                udpClient.Connect((IPEndPoint)tcpClient.Client.RemoteEndPoint);
                Client clientobj = new Client(tcpClient, udpClient, nextClientID);

                //add the client to the lobby
                try
                {
                    lobby.AddClient(clientobj);
                }
                catch (Lobby.ClientsLockedException)
                {
                    // Do nothing for now.
                }
                nextClientID++;
            }
        }
    }
}
