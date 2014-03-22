#region Using Statements
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
#endregion

namespace MyClient
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static ThreadSafeQueue<GameMessage> outgoingQue = new ThreadSafeQueue<GameMessage>();
        private static ThreadSafeQueue<GameMessage> inCommingQue = new ThreadSafeQueue<GameMessage>();
        private static Vector2 worldSize;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            GameMessage.Initialize();

            //TODO: find ip address

            //Microst.VisualBasic.Interaction.InputBox("Did you know your question goes here?", "Title", "Default Text");
            string serverIP = Microsoft.VisualBasic.Interaction.InputBox("Enter Server IP Address", "Server IP Address", "127.0.0.1");

            IPAddress address = IPAddress.Parse(serverIP);

            Console.WriteLine("connecting to: " + address.ToString());

            //Connect to the server
            TcpClient prelimTcpClient = new TcpClient();
            IPEndPoint prelimServerEndPoint = new IPEndPoint(address, 3000);
            prelimTcpClient.Connect(prelimServerEndPoint);

            Client prelimClient = new Client(prelimTcpClient, null, 0);

            //get a port assignment
            GameMessage message = GameMessage.ReadTCPMessage(prelimClient);
            if (!(message is ClientID))
            {
                throw new Exception("Client needs port assignment");
            }

            //close the preliminary port
            prelimTcpClient.Close();

            //connect to the assigned port
            ClientID portMessage = (ClientID)message;
            TcpClient tcpclient = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(address, portMessage.ID + 3000);
            tcpclient.Connect(serverEndPoint);
            UdpClient udpClient = new UdpClient((IPEndPoint)tcpclient.Client.LocalEndPoint);
            udpClient.Connect((IPEndPoint)tcpclient.Client.RemoteEndPoint);
            Client client = new Client(tcpclient, udpClient, portMessage.ID);

            //get the world size
            GameMessage m = GameMessage.ReadTCPMessage(client);
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

            StartGame(client.GetID());

            while (true)
            {
                GameMessage ingMessage = outgoingQue.Dequeue();
                client.SendUDPMessage(ingMessage);
            }
        }

        private static void StartGame(Int32 playerID)
        {
            Thread gameThread = new Thread(new ParameterizedThreadStart(RunGame));
            gameThread.Start(playerID);

        }

        private static void RunGame(object obj)
        {
            Int32 playerID = (Int32)obj;
            using (var game = new MyGame.Game1(outgoingQue, inCommingQue, playerID, worldSize))
                game.Run();
        }

        
        private static void InboundReader(object obj)
        {
            Client client = (Client)obj;

            while (client.IsConnected())
            {
                GameMessage m = GameMessage.ReadUDPMessage(client);
                inCommingQue.Enqueue(m);
            }
        }

    }
#endif
}
