using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using NetworkingLibrary;
using Microsoft.Xna.Framework;

namespace Server
{
    class Lobby
    {
        private class ClientStatePair
        {
            public Client client;
        }
        private float hexSize = 900;

        private uint nextTeam = 1;
        private uint nextUnit = 1;

        private bool clientsLocked = false;
        private List<Client> clients;
        private Mutex clientsMutex;
        private Semaphore clientsCanged;



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

        private void printLobbyChanges(object obj)
        {
            //Application.Run(form);
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
                ClientStatePair pair = new ClientStatePair();
                pair.client = c;

                Thread clientThread = new Thread(new ParameterizedThreadStart(ClientCom));
                clientThread.Start(pair);
            }

           /*
            while (true)
            {
                TcpMessage message = state.OutboundMessageDequeue();
                foreach (Client c in clients)
                {
                    c.WriteMessage(message);
                }
            }*/

        }

        private static void StartGame()
        {
            /*
            Thread gameThread = new Thread(new ParameterizedThreadStart(RunGame));
            gameThread.Start();
             */
        }

        private static void ClientCom(object obj)
        {
            ClientStatePair pair = (ClientStatePair)obj;
            Client client = pair.client;

           /* while (true)
            {
                state.AddMessages(client.Read());
            } */
        }

        
/*
        private void SendToAllClients(TcpMessage message)
        {
            foreach (Client c in clients)
            {
                c.WriteMessage(message);
            }
        }*/


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
