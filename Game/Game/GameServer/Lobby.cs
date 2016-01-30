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
using MyGame.GameClient;
using MyGame.Utils;

namespace MyGame.GameServer
{
    public class Lobby
    {
        private volatile bool clientsLocked = false;
        private List<Player> clients = new List<Player>();
        private Mutex clientsMutex = new Mutex(false);

        public List<Player> Clients
        {
            get
            {
                return clients;
            }
        }

        // Adds a client to the current clientlist. Returns true if the client is added, returns false if the clients are locked.
        private bool AddClient(Player client)
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
            foreach (Player c in clients)
            {
                c.Disconnect();
            }

            //this.prelimListener.Stop();
            UdpTcpPair.StopListener();
            clientListenerThread.Join();
        }

        public void BroadcastUDP(ClientUpdate message)
        {
            foreach (Player client in clients)
            {
                client.SendUDP(message);
            }
        }

        public void BroadcastUDP(Queue<ClientUpdate> messages)
        {
            foreach (Player client in clients)
            {
                client.SendUDP(messages);
            }
        }

        public void BroadcastTCP(ClientUpdate message)
        {
            foreach (Player client in clients)
            {
                client.SendTCP(message);
            }
        }

        private void ClientListener()
        {
            UdpTcpPair.InitializeListener();

            while (true)
            {
                Player clientobj = new Player(this);

                //add the client to the lobby
                this.AddClient(clientobj);
            }

        }

        public void Update()
        {
            foreach (Player client in clients)
            {
                client.UpdateControlState();
            }
        }

    }
}
