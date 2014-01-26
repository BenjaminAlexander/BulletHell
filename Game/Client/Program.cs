using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkingLibrary;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace MyClient
{
    class Program
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
            bool fastConnect = true;

            //TODO: find ip address
            IPAddress address = IPAddress.Parse("127.0.0.1");


            Console.WriteLine("connecting to: " + address.ToString());
            TcpClient tcpclient = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(address, 3000);

            tcpclient.Connect(serverEndPoint);

            Client client = new Client(tcpclient);

            

            ClientStatePair csp = new ClientStatePair();
            csp.client = client;
            /*
            Thread outThread = new Thread(new ParameterizedThreadStart(OutboundSender));
            outThread.Start(csp);

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
        }

        
        private static void OutboundSender(object obj)
        {
            ClientStatePair csp = (ClientStatePair)obj;
            Client client = csp.client;
            GameState state = csp.state;

            while (true)
            {
                client.WriteMessage(state.OutboundMessageDequeue());
            }
        }*/
    }
}
