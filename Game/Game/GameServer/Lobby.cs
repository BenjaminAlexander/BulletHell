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
using MyGame.GameClient;

namespace MyGame.GameServer
{
    public class Lobby
    {
        private int nextClientID = 1;
        private TcpListener prelimListener;

        private volatile bool clientsLocked = false;
        private List<LobbyClient> clients = new List<LobbyClient>();
        private Mutex clientsMutex = new Mutex(false);

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

            // Start up the game.
            Game1 game = new ServerGame(this);
            game.Run();

            //the game has finished
            foreach (LobbyClient c in clients)
            {
                c.Disconnect();
            }

            this.prelimListener.Stop();
            clientListenerThread.Join();
        }

        public void BroadcastUDP(ClientUpdate message)
        {
            foreach (LobbyClient client in clients)
            {
                client.SendUDP(message);
            }
        }

        public void BroadcastUDP(Queue<ClientUpdate> messages)
        {
            foreach (LobbyClient client in clients)
            {
                client.SendUDP(messages);
            }
        }

        public void BroadcastTCP(ClientUpdate message)
        {
            foreach (LobbyClient client in clients)
            {
                client.SendTCP(message);
            }
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
                    (new ClientID(nextClientID)).SendTCP(prelimClient.GetStream());
                    prelimClient.Close();

                    LobbyClient clientobj = new LobbyClient(this, nextClientID + 3000, nextClientID);

                    //add the client to the lobby
                    this.AddClient(clientobj);
                    nextClientID++;
                }
            }
            catch (Exception) { }
        }

        public void Update()
        {
            foreach (LobbyClient client in clients)
            {
                client.UpdateControlState();
            }
        }

    }
}
