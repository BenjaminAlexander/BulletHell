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
using MyGame.PlayerControllers;

namespace MyGame.GameServer
{
    public class Lobby
    {
        private int nextClientID = 1;
        private TcpListener prelimListener;

        private volatile bool clientsLocked = false;
        private List<LobbyClient> clients = new List<LobbyClient>();
        private Mutex clientsMutex = new Mutex(false);

        private ThreadSafeQueue<GameMessage> outgoingUDPQueue = new ThreadSafeQueue<GameMessage>();
        private ThreadSafeQueue<GameMessage> outgoingTCPQueue = new ThreadSafeQueue<GameMessage>();
        private ThreadSafeQueue<ServerUpdate> incomingQueue = new ThreadSafeQueue<ServerUpdate>();

        public List<LobbyClient> Clients
        {
            get
            {
                return clients;
            }
        }

        // Adds a client to the current clientlist. Returns true if the client is added, returns false if the clients are locked.
        private bool AddClient(LobbyClient client)
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

        public void EnqueueInboundMessage(ServerUpdate message)
        {
            incomingQueue.Enqueue(message);
        }

        public void EnqueueInboundMessage(Queue<ServerUpdate> messages)
        {
            incomingQueue.EnqueueAll(messages);
        }

        public ServerUpdate DequeueInboundMessage()
        {
            return incomingQueue.Dequeue();
        }

        public Queue<ServerUpdate> DequeueAllInboundMessages()
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

                    LobbyClient clientobj = new LobbyClient(this, nextClientID + 3000, nextClientID);

                    //add the client to the lobby
                    this.AddClient(clientobj);
                    nextClientID++;
                }
            }
            catch (Exception) { }
        }

        //TODO: clean this up to be threadsafe mostly
        public List<int> ControllersIDs
        {
            get
            {
                List<int> rtn = new List<int>();
                foreach (LobbyClient client in clients)
                {
                    rtn.Add(client.GetID());
                }
                return rtn;
            }
        }

        public NetworkPlayerController this[int id]
        {
            get
            {
                NetworkPlayerController rtn = null;
                foreach (LobbyClient client in clients)
                {
                    if(client.GetID() == id)
                    {
                        rtn = client.Controller;
                    }
                }
                return rtn;
            }
        }

    }
}
