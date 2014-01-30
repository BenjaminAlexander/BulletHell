using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyServer
{
    class Lobby
    {
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

        private bool clientsLocked = false;
        private List<Client> clients;
        private Mutex clientsMutex;
        private Semaphore clientsCanged;

        private ThreadSafeQue<TCPMessage> outgoingQue = new ThreadSafeQue<TCPMessage>();
        private ThreadSafeQue<TCPMessage> inCommingQue = new ThreadSafeQue<TCPMessage>(); 


        public Lobby()
        {
            this.clients = new List<Client>();
            clientsMutex = new Mutex(false);
            clientsCanged = new Semaphore(0, Int32.MaxValue);
        }

        public void AddClient(Client client)
        {
            clientsMutex.WaitOne();
            if (!clientsLocked)
            {
                clients.Add(client);

                Console.WriteLine("Client List");
                foreach (Client c in clients)
                {
                    Console.WriteLine(c.GetID() + ": is connected? " + c.IsConnected());
                }

                clientsCanged.Release();
            }
            clientsMutex.ReleaseMutex();
        }

        public void StartLobby()
        {
            Thread clientThread = new Thread(new ParameterizedThreadStart(runLobby));
            clientThread.Start(null);
        }

        private void runLobby(object obj)
        {
            //wait to start
            MessageBox.Show("Enter to start", GetLocalIP());

            clientsMutex.WaitOne();
            clientsLocked = true;
            clientsMutex.ReleaseMutex();

            MyGame.GameStateObjects.GameObject.InitializeGameObjects(new Vector2(10));
            
            foreach (Client c in clients)
            {
                ClientQuePair pair = new ClientQuePair();
                pair.client = c;
                pair.inCommingQue = inCommingQue;

                Thread clientThread = new Thread(new ParameterizedThreadStart(ClientCom));
                clientThread.Start(pair);
            }

            Vector2 worldSize = new Vector2(20000);
            SendToAllClients(new SetWorldSize(worldSize));

            StartGame();

            //Get messages from state and broadcast
            while (true)
            {
                outgoingQue.WaitOn();
                SendToAllClients(outgoingQue.Dequeue());
            }

        }

        private void StartGame()
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
            using (var game = new MyGame.Game1(pair.outgoingQue, pair.inCommingQue, true))
                game.Run();
        }

        private static void ClientCom(object obj)
        {
            ClientQuePair pair = (ClientQuePair)obj;
            Client client = pair.client;
            ThreadSafeQue<TCPMessage> inCommingQue = pair.inCommingQue;


            while (client.IsConnected())
            {
                TCPMessage m = TCPMessage.ReadMessage(client);
                Console.WriteLine(m.GetType().ToString());
                inCommingQue.Enqueue(m);
            } 
        }

        

        private void SendToAllClients(TCPMessage message)
        {
            foreach (Client c in clients)
            {
                c.SendMessage(message);
            }
        }


        private string GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        
    }
}
