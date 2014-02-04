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
#endregion

namespace MyClient
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private class ClientStatePair
        {
            public Client client;

        }

        private class QueuePair
        {
            public ThreadSafeQueue<TCPMessage> outgoingQue;
            public ThreadSafeQueue<TCPMessage> inCommingQue;
        }

        private class ClientQueuePair
        {
            public Client client;
            public ThreadSafeQueue<TCPMessage> inCommingQue;
        }

        private static ThreadSafeQueue<TCPMessage> outgoingQue = new ThreadSafeQueue<TCPMessage>();
        private static ThreadSafeQueue<TCPMessage> inCommingQue = new ThreadSafeQueue<TCPMessage>(); 

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            TCPMessage.Initialize();
            //TODO: find ip address
            IPAddress address = IPAddress.Parse("127.0.0.1");

            Console.WriteLine("connecting to: " + address.ToString());
            TcpClient tcpclient = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(address, 3000);

            tcpclient.Connect(serverEndPoint);
            UdpClient udpClient = new UdpClient((IPEndPoint)tcpclient.Client.LocalEndPoint);
            udpClient.Connect((IPEndPoint)tcpclient.Client.RemoteEndPoint);

            Client client = new Client(tcpclient, udpClient);

            /*
            ClientStatePair csp = new ClientStatePair();
            csp.client = client;

            Thread outThread = new Thread(new ParameterizedThreadStart(OutboundSender));
            outThread.Start(csp);
            */

            
            TCPMessage m = TCPMessage.ReadTCPMessage(client);
            if (m is SetWorldSize)
            {
                //TODO: right now it does nothing with the world size.  It is currently hard coded in
            }
            else
            {
                throw new Exception("Client needs world size first");
            }


            ClientQueuePair pair = new ClientQueuePair();
            pair.client = client;
            pair.inCommingQue = inCommingQue;

            Thread clientThread = new Thread(new ParameterizedThreadStart(InboundReader));
            clientThread.Start(pair);

            StartGame();

            while (true)
            {
                outgoingQue.WaitOn();
                m = outgoingQue.Dequeue();
                client.SendUDPMessage(m);
            }
        }

        private static void StartGame()
        {
            QueuePair pair = new QueuePair();
            pair.outgoingQue = outgoingQue;
            pair.inCommingQue = inCommingQue;

            Thread gameThread = new Thread(new ParameterizedThreadStart(RunGame));
            gameThread.Start(pair);

        }

        private static void RunGame(object obj)
        {
            QueuePair pair = (QueuePair)obj;
            using (var game = new MyGame.Game1(pair.outgoingQue, pair.inCommingQue, false))
                game.Run();
        }

        
        private static void InboundReader(object obj)
        {
            ClientQueuePair pair = (ClientQueuePair)obj;
            Client client = pair.client;
            ThreadSafeQueue<TCPMessage> inCommingQue = pair.inCommingQue;


            while (client.IsConnected())
            {
                //Console.WriteLine("start");
                TCPMessage m = TCPMessage.ReadUDPMessage(client);
                //Console.WriteLine(m.GetType().ToString());
                inCommingQue.Enqueue(m);
                     //Console.WriteLine("stop");
            }
            Console.WriteLine("Client inbound is dead");
        }

    }
#endif
}
