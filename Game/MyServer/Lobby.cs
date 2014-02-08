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

        private bool clientsLocked = false;
        private List<Client> clients;
        private Mutex clientsMutex;
        private Semaphore clientsCanged;

        private ThreadSafeQueue<TCPMessage> outgoingQue = new ThreadSafeQueue<TCPMessage>();
        private ThreadSafeQueue<TCPMessage> inCommingQue = new ThreadSafeQueue<TCPMessage>(); 


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
            
            foreach (Client c in clients)
            {
                ClientQueuePair pair = new ClientQueuePair();
                pair.client = c;
                pair.inCommingQue = inCommingQue;

                Thread clientThread = new Thread(new ParameterizedThreadStart(ClientCom));
                clientThread.Start(pair);
            }

            Vector2 worldSize = new Vector2(20000);
            SendTCPToAllClients(new SetWorldSize(worldSize));

            StartGame();

            //Get messages from state and broadcast
            while (true)
            {
                outgoingQue.WaitOn();
                SendUDPToAllClients(outgoingQue.Dequeue());
            }

        }

        private void StartGame()
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
            using (var game = new MyGame.Game1(pair.outgoingQue, pair.inCommingQue, true, 0))
                game.Run();
        }

        private static void ClientCom(object obj)
        {
            ClientQueuePair pair = (ClientQueuePair)obj;
            Client client = pair.client;
            ThreadSafeQueue<TCPMessage> inCommingQue = pair.inCommingQue;


            while (client.IsConnected())
            {
                TCPMessage m = TCPMessage.ReadUDPMessage(client);
                inCommingQue.Enqueue(m);
            } 
        }

        

        private void SendTCPToAllClients(TCPMessage message)
        {
            foreach (Client c in clients)
            {
                c.SendTCPMessage(message);
            }
        }

        private void SendUDPToAllClients(TCPMessage message)
        {
            foreach (Client c in clients)
            {
                c.SendUDPMessage(message);
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
