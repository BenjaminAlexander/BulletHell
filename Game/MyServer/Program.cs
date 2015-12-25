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
        private static TcpListener prelimListener;
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

            //Start the client listener in its own thread
            prelimListener = new TcpListener(IPAddress.Any, 3000);
            Thread clientListenerThread = new Thread(new ThreadStart(ClientListener));
            clientListenerThread.Start();

            //wait for the game(contained in the lobby) to end
            lobby.Join();

            //close down the client listener once the game is over
            prelimListener.Stop();
            clientListenerThread.Join();
        }

        private static void ClientListener()
        {
            prelimListener.Start();
            try
            {
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
                    lobby.AddClient(clientobj);
                    nextClientID++;
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
