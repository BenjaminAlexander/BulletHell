#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using MyNetworkLibrary;
using MyGame;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
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

            Client client = new Client(tcpclient);



            ClientStatePair csp = new ClientStatePair();
            csp.client = client;

            Thread outThread = new Thread(new ParameterizedThreadStart(OutboundSender));
            outThread.Start(csp);
            /*
            while (true)
            {
                state.AddMessages(client.Read());
            }*/
        }

        /*
        private static void StartGame(WindowsGame1.GameState state)
        {
            Thread gameThread = new Thread(new ParameterizedThreadStart(RunGame));
            gameThread.Start(state);

        }

        private static void RunGame(object obj)
        {
            WindowsGame1.GameState state = (WindowsGame1.GameState)obj;
            using (WindowsGame1.Game1 game = new WindowsGame1.Game1(state))
            {
                game.Run();
            }
        }*/


        private static void OutboundSender(object obj)
        {
            ClientStatePair csp = (ClientStatePair)obj;
            Client client = csp.client;

            int i = 0;
            while (true)
            {
                //client.WriteBuffer(BitConverter.GetBytes(i++), 4);
                TCPMessage m = new SetWorldSize(new Vector2(23));
                client.SendMessage(m);
                //TODO: get messages and send them here
                //client.WriteMessage(state.OutboundMessageDequeue());
            }
        }
    }
#endif
}
