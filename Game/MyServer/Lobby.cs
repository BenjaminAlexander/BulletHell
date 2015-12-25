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

namespace MyServer
{
    class Lobby
    {
        private bool clientsLocked = false;
        private List<Client> clients = new List<Client>();
        private Mutex clientsMutex = new Mutex(false);
        private Semaphore clientsCanged = new Semaphore(0, Int32.MaxValue);

        private ThreadSafeQueue<GameMessage> outgoingQueue = new ThreadSafeQueue<GameMessage>();
        private ThreadSafeQueue<GameMessage> incomingQueue = new ThreadSafeQueue<GameMessage>();

        //TODO: the lobby seems like the wrong place for the world size
        private Vector2 worldSize;

        private Thread clientThread = null;

        public Lobby()
        {
            this.clientThread = new Thread(new ThreadStart(RunLobby));
            this.clientThread.Start();
        }

        // Adds a client to the current clientlist. Returns true if the client is added, returns false if the clients are locked.
        public bool AddClient(Client client)
        {
            clientsMutex.WaitOne();

            // Inform the caller that the clients are currently locked.
            if (clientsLocked)
            {
                clientsMutex.ReleaseMutex();
                return false;
            }
            else
            {           
                clients.Add(client);
                StaticNetworkPlayerManager.Add(client.GetID());

                //TODO: we need a better way to keep a debug log
                Console.WriteLine("Client List");
                foreach (Client c in clients)
                {
                    Console.WriteLine(c.GetID() + ": is connected? " + c.IsConnected());
                }

                clientsCanged.Release();
                clientsMutex.ReleaseMutex();
                return true;
            }
            
        }

        public void Join()
        {
            this.clientThread.Join();
        }

        private void RunLobby()
        {
            //wait to start
            MessageBox.Show("Enter to start", NetUtils.GetLocalIP());

            clientsMutex.WaitOne();
            clientsLocked = true;
            clientsMutex.ReleaseMutex();

            List<Thread> clientThreadList = new List<Thread>(); 
            foreach (Client c in clients)
            {
                Thread clientThread = new Thread(new ParameterizedThreadStart(InboundClientReader));
                clientThread.Start(c);
                clientThreadList.Add(clientThread);
            }

            worldSize = new Vector2(20000);
            SendTCPToAllClients(new SetWorldSize(worldSize));

            // Start up the game.
            Game1 game = new Game1(outgoingQueue, incomingQueue, 0, worldSize);
            Thread gameThread = new Thread(new ThreadStart(game.Run));
            gameThread.Start();

            Thread outboundReaderThread = new Thread(new ThreadStart(OutboundReader));
            outboundReaderThread.Start();

            gameThread.Join();

            foreach (Thread c in clientThreadList)
            {
                c.Abort();
            }
            outboundReaderThread.Abort();

        }

        private void OutboundReader()
        {
            //Get messages from state and broadcast
            while (true)
            {
                SendUDPToAllClients(outgoingQueue.Dequeue());
            }
        }

        private void InboundClientReader(object obj)
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
            // The thread is ending, this client is done listening.  Get rid of the client from the lobby's client list.
            //TODO: this doesn't seem thread safe
            clients.Remove(client);
        }

        public void BroadcastUDP(GameMessage message)
        {
            outgoingQueue.Enqueue(message);
        }

        public GameMessage DequeInboundMessage()
        {
            return incomingQueue.Dequeue();
        }

        private void SendTCPToAllClients(GameMessage message)
        {
            foreach (Client c in clients)
            {
                c.SendTCPMessage(message);
            }
        }

        private void SendUDPToAllClients(GameMessage message)
        {
            foreach (Client c in clients)
            {
                c.SendUDPMessage(message);
            }
        }
    }
}
