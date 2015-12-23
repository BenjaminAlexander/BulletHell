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
        private Vector2 worldSize;

        private Thread clientThread = null;

        // Adds a client to the current clientlist. Throws ClientsLockedException if the clients are locked.
        public void AddClient(Client client)
        {
            clientsMutex.WaitOne();

            // Inform the caller that the clients are currently locked.
            if (clientsLocked)
            {
                clientsMutex.ReleaseMutex();
                throw new ClientsLockedException();
            }

            clients.Add(client);
            StaticNetworkPlayerManager.Add(client.GetID());

            Console.WriteLine("Client List");
            foreach (Client c in clients)
            {
                Console.WriteLine(c.GetID() + ": is connected? " + c.IsConnected());
            }

            clientsCanged.Release();
            clientsMutex.ReleaseMutex();
        }

        public void StartLobby()
        {
            this.clientThread = new Thread(new ParameterizedThreadStart(RunLobby));
            this.clientThread.Start(null);
        }

        public void Join()
        {
            this.clientThread.Join();
        }

        private void RunLobby(object obj)
        {
            //wait to start
            MessageBox.Show("Enter to start", NetUtils.GetLocalIP());

            clientsMutex.WaitOne();
            clientsLocked = true;
            clientsMutex.ReleaseMutex();
            
            foreach (Client c in clients)
            {
                Thread clientThread = new Thread(new ParameterizedThreadStart(ClientCom));
                clientThread.Start(c);
            }

            worldSize = new Vector2(20000);
            SendTCPToAllClients(new SetWorldSize(worldSize));

            // Start up the game.
            Thread gameThread = new Thread(new ParameterizedThreadStart(RunGame));
            gameThread.Start(null);

            //Get messages from state and broadcast
            while (true)
            {
                SendUDPToAllClients(outgoingQueue.Dequeue());
            }

        }

        private void RunGame(object obj)
        {
            using (var game = new MyGame.Game1(outgoingQueue, incomingQueue, 0, worldSize))
                game.Run();
        }

        private void ClientCom(object obj)
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
            clients.Remove(client);
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

        // Exception thrown by AddClient when the clients are locked.
        public class ClientsLockedException : Exception { }
    }
}
