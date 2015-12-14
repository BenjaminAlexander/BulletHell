using System;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using Microsoft.VisualBasic;

namespace MyClient
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static ThreadSafeQueue<GameMessage> outgoingQueue = new ThreadSafeQueue<GameMessage>();
        private static ThreadSafeQueue<GameMessage> incomingQueue = new ThreadSafeQueue<GameMessage>();
        private static Vector2 worldSize;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            GameMessage.Initialize();

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

            Console.WriteLine("connecting to: " + address.ToString());
           
            int clientID = GetClientID(address);

            if (clientID == 0)
            {
                return;
            }

            TcpClient tcpclient = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(address, clientID + 3000);
            tcpclient.Connect(serverEndPoint);
            UdpClient udpClient = new UdpClient((IPEndPoint)tcpclient.Client.LocalEndPoint);
            udpClient.Connect((IPEndPoint)tcpclient.Client.RemoteEndPoint);
            Client client = new Client(tcpclient, udpClient, clientID);

            // Attempt to get the world size.
            GameMessage m;
            try
            {
                m = client.ReadTCPMessage();
            }
            catch (Client.ClientNotConnectedException)
            {
                throw new Exception("Client Disconnected.");
            }

            if (m is SetWorldSize)
            {
                //TODO: right now it does nothing with the world size.  It is currently hard coded in
                worldSize = ((SetWorldSize)m).Size;
            }
            else
            {
                throw new Exception("Client needs world size first");
            }

            Thread clientThread = new Thread(new ParameterizedThreadStart(InboundReader));
            clientThread.Start(client);

            Thread gameThread = StartGame(client.GetID());

            while (gameThread.IsAlive)
            {
                GameMessage ingMessage = outgoingQueue.Dequeue();
                client.SendUDPMessage(ingMessage);
            }

            return;
        }

        private static Thread StartGame(Int32 playerID)
        {
            Thread gameThread = new Thread(new ParameterizedThreadStart(RunGame));
            gameThread.Start(playerID);
            return gameThread;
        }

        private static void RunGame(object obj)
        {
            Int32 playerID = (Int32)obj;
            using (var game = new MyGame.Game1(outgoingQueue, incomingQueue, playerID, worldSize))
                game.Run();
        }

        
        private static void InboundReader(object obj)
        {
            Client client = (Client)obj;

            while (client.IsConnected())
            {
                GameMessage m;
                try
                {
                    m = client.ReadUDPMessage();
                    incomingQueue.Enqueue(m);
                }
                catch (Client.ClientNotConnectedException)
                {
                    // Do nothing.  The client will disconnect quietly.
                    // TODO:  What else do we need to do here to clean up a client disconnect?
                }
            }
        }

        //GetClientID sets up a TCP connection with the server.  
        //The server then assigns the client an integer ID.  
        //The client then closes the connection and uses the ID to 
        //set up the connection to the server.  This allows multiple 
        //clients to connect to the same server using non-colliding ports.
        private static int GetClientID(IPAddress serverIP)
        {
            try
            {
                //Connect to the server
                TcpClient prelimTcpClient = new TcpClient();
                IPEndPoint prelimServerEndPoint = new IPEndPoint(serverIP, 3000);
                prelimTcpClient.Connect(prelimServerEndPoint);

                Client prelimClient = new Client(prelimTcpClient, null, 0);

                // Attempt to get the port assignment.
                GameMessage message = prelimClient.ReadTCPMessage();

                //close the preliminary port
                prelimTcpClient.Close();

                ClientID portMessage = (ClientID)message;
                return portMessage.ID;

            }
            catch (Exception)
            {
                return 0;
            }
        }

    }
}
