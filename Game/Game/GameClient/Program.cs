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

namespace MyGame.GameClient
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static ThreadSafeQueue<GameMessage> outgoingQueue = new ThreadSafeQueue<GameMessage>();
        private static ThreadSafeQueue<GameMessage> incomingQueue = new ThreadSafeQueue<GameMessage>();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void ClientMain()
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
           
            int clientID = GetClientID(address);
            if (clientID == 0)
            {
                return;
            }

            Client client = new Client(address, clientID + 3000, clientID);

            Thread inboundTCPReaderThread = new Thread(new ParameterizedThreadStart(InboundTCPReader));
            inboundTCPReaderThread.Start(client);

            Thread inboundUDPReaderThread = new Thread(new ParameterizedThreadStart(InboundUDPReader));
            inboundUDPReaderThread.Start(client);

            Thread outboundReaderThread = new Thread(new ParameterizedThreadStart(OutboundReader));
            outboundReaderThread.Start(client);

            ClientGame game = new ClientGame(outgoingQueue, incomingQueue, client.GetID());
            game.Run();

            client.Disconnect();
            inboundTCPReaderThread.Abort();
            inboundUDPReaderThread.Abort();
            outboundReaderThread.Abort();

            return;
        }

        private static void InboundUDPReader(object obj)
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

        private static void InboundTCPReader(object obj)
        {
            Client client = (Client)obj;

            while (client.IsConnected())
            {
                GameMessage m;
                try
                {
                    m = client.ReadTCPMessage();
                    incomingQueue.Enqueue(m);
                }
                catch (Client.ClientNotConnectedException)
                {
                    // Do nothing.  The client will disconnect quietly.
                    // TODO:  What else do we need to do here to clean up a client disconnect?
                }
            }
        }

        private static void OutboundReader(object obj)
        {
            Client client = (Client)obj;

            while (client.IsConnected())
            {

                GameMessage m = outgoingQueue.Dequeue();
                client.SendUDPMessage(m);
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

                // Attempt to get the port assignment.
                GameMessage message = NetUtils.ReadTCPMessage(prelimTcpClient.GetStream());

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
