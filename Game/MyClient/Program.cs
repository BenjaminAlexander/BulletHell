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

        private class QuePair
        {
            public ThreadSafeQue<TCPMessage> outgoingQue;
            public ThreadSafeQue<TCPMessage> inCommingQue;
        }

        private class ClientQuePair
        {
            public Client client;
            public ThreadSafeQue<TCPMessage> inCommingQue;
        }

        private static ThreadSafeQue<TCPMessage> outgoingQue = new ThreadSafeQue<TCPMessage>();
        private static ThreadSafeQue<TCPMessage> inCommingQue = new ThreadSafeQue<TCPMessage>(); 

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
                MyGame.GameStateObjects.GameObject.InitializeGameObjects(((SetWorldSize)m).Size);
            }
            else
            {
                throw new Exception("Client needs world size first");
            }


            ClientQuePair pair = new ClientQuePair();
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
            QuePair pair = new QuePair();
            pair.outgoingQue = outgoingQue;
            pair.inCommingQue = inCommingQue;

            Thread gameThread = new Thread(new ParameterizedThreadStart(RunGame));
            gameThread.Start(pair);

        }

        private static void RunGame(object obj)
        {
            QuePair pair = (QuePair)obj;
            using (var game = new MyGame.Game1(pair.outgoingQue, pair.inCommingQue, false))
                game.Run();
        }

        
        private static void InboundReader(object obj)
        {
            ClientQuePair pair = (ClientQuePair)obj;
            Client client = pair.client;
            ThreadSafeQue<TCPMessage> inCommingQue = pair.inCommingQue;


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
