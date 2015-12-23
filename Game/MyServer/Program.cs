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
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static int nextClientID = 1;
        private static Lobby lobby;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GameMessage.Initialize();

            lobby = new Lobby();
            lobby.StartLobby();

            Thread ClientListenerThread = new Thread(new ThreadStart(ClientListener));
            ClientListenerThread.Start();

            lobby.Join();

            ClientListenerThread.Abort();
        }

        private static void ClientListener()
        {
            TcpListener prelimListener;
            prelimListener = new TcpListener(IPAddress.Any, 3000);
            prelimListener.Start();

            while (true)
            {
                //Listen, connect, and then send the new client its ID, then disconnect
                TcpClient prelimClient = prelimListener.AcceptTcpClient();
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
