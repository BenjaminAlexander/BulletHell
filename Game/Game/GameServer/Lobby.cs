using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using MyGame;
using MyGame.Networking;

namespace MyGame.GameServer
{
    class Lobby
    {
        private int nextClientID = 1;
        private TcpListener prelimListener;

        private volatile bool clientsLocked = false;
        private List<Client> clients = new List<Client>();
        private Mutex clientsMutex = new Mutex(false);

        private ThreadSafeQueue<GameMessage> outgoingUDPQueue = new ThreadSafeQueue<GameMessage>();
        private ThreadSafeQueue<GameMessage> outgoingTCPQueue = new ThreadSafeQueue<GameMessage>();
        private ThreadSafeQueue<GameMessage> incomingQueue = new ThreadSafeQueue<GameMessage>();

        public List<Client> Clients
        {
            get
            {
                return clients;
            }
        }

        // Adds a client to the current clientlist. Returns true if the client is added, returns false if the clients are locked.
        public bool AddClient(Client client)
        {
            bool added = false;
            clientsMutex.WaitOne();

            if (!clientsLocked)
            {
                clients.Add(client);
                added = true;
            }

            clientsMutex.ReleaseMutex();
            return added;
        }

        //This method starts running the lobby and blocks until the lobby ends
        public void Run()
        {
            //Start the client listener in its own thread
            //This thread allows clients to asynchronously join the lobby
            Thread clientListenerThread = new Thread(new ThreadStart(ClientListener));
            clientListenerThread.Start();

            //wait to start
            MessageBox.Show("Enter to start", NetUtils.GetLocalIP());

            //lock client list
            clientsMutex.WaitOne();
            clientsLocked = true;
            clientsMutex.ReleaseMutex();

            //start threads which collect inbound messages and place them in the incomingQueue
            foreach (Client c in clients)
            {
                Thread clientUDPThread = new Thread(new ParameterizedThreadStart(InboundUDPClientReader));
                clientUDPThread.Start(c);

                Thread clientTCPThread = new Thread(new ParameterizedThreadStart(InboundTCPClientReader));
                clientTCPThread.Start(c);
            }

            Thread outboundUDPSenderThread = new Thread(new ThreadStart(OutboundUDPSender));
            outboundUDPSenderThread.Start();

            Thread outboundTCPSenderThread = new Thread(new ThreadStart(OutboundTCPSender));
            outboundTCPSenderThread.Start();

            // Start up the game.
            Game1 game = new ServerGame(this);
            game.Run();

            //the game has finished
            foreach (Client c in clients)
            {
                c.Disconnect();
            }

            outboundUDPSenderThread.Abort();
            outboundTCPSenderThread.Abort();

            this.prelimListener.Stop();
            clientListenerThread.Join();
        }

        private void OutboundUDPSender()
        {
            //Get messages from outgoing queue and broadcast
            while (true)
            {
                GameMessage message = outgoingUDPQueue.Dequeue();
                foreach (Client c in clients)
                {
                    c.SendUDPMessage(message);
                }
            }
        }

        private void OutboundTCPSender()
        {
            //Get messages from state and broadcast
            while (true)
            {
                GameMessage message = outgoingTCPQueue.Dequeue();
                foreach (Client c in clients)
                {
                    c.SendTCPMessage(message);
                }
            }
        }

        private void InboundUDPClientReader(object obj)
        {
            Client client = (Client)obj;
            try
            {
                while (client.IsConnected())
                {
                    GameMessage m = client.ReadUDPMessage();
                    incomingQueue.Enqueue(m);
                }
            }
            catch (Exception) { }
            // The thread is ending, this client is done listening.
        }

        private void InboundTCPClientReader(object obj)
        {
            Client client = (Client)obj;
            try
            {
                while (client.IsConnected())
                {
                    GameMessage m = client.ReadTCPMessage();
                    incomingQueue.Enqueue(m);
                }
            }
            catch (Exception) { }
            // The thread is ending, this client is done listening.
        }

        public void BroadcastUDP(GameMessage message)
        {
            outgoingUDPQueue.Enqueue(message);
        }

        public void BroadcastUDP(Queue<GameMessage> messages)
        {
            outgoingUDPQueue.EnqueueAll(messages);
        }

        public void BroadcastTCP(GameMessage message)
        {
            outgoingTCPQueue.Enqueue(message);
        }

        public GameMessage DequeueInboundMessage()
        {
            return incomingQueue.Dequeue();
        }

        public Queue<GameMessage> DequeueAllInboundMessages()
        {
            return incomingQueue.DequeueAll();
        }

        private void ClientListener()
        {
            this.prelimListener = new TcpListener(IPAddress.Any, 3000);
            this.prelimListener.Start();
            try
            {
                while (true)
                {

                    //Listen, connect, and then send the new client its ID, then disconnect
                    TcpClient prelimClient = prelimListener.AcceptTcpClient();
                    (new ClientID(nextClientID)).SendTCP(prelimClient.GetStream(), new Mutex());
                    prelimClient.Close();

                    Client clientobj = new Client(nextClientID + 3000, nextClientID);

                    //add the client to the lobby
                    this.AddClient(clientobj);
                    nextClientID++;
                }
            }
            catch (Exception) { }
        }
    }
}
