using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using System.Net;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameStateObjects;
using System.Threading;

namespace MyGame.GameClient
{
    //TODO: name this class better
    //maybe combine with local player
    public class ServerConnection
    {
        private UdpTcpPair client;

        private static ThreadSafeQueue<PlayerControllerUpdate> outgoingQueue = new ThreadSafeQueue<PlayerControllerUpdate>();
        private static ThreadSafeQueue<GameObjectUpdate> incomingUDPQueue = new ThreadSafeQueue<GameObjectUpdate>();
        //private static ThreadSafeQueue<ClientUpdate> incomingTCPQueue = new ThreadSafeQueue<ClientUpdate>();

        //private Thread inboundTCPReaderThread;
        private Thread inboundUDPReaderThread;
        private Thread outboundReaderThread;

        public ServerConnection(IPAddress serverAddress)
        {
            this.client = new UdpTcpPair(serverAddress);

            //inboundTCPReaderThread = new Thread(new ThreadStart(InboundTCPReader));
            //inboundTCPReaderThread.Start();

            inboundUDPReaderThread = new Thread(new ThreadStart(InboundUDPReader));
            inboundUDPReaderThread.Start();

            outboundReaderThread = new Thread(new ThreadStart(OutboundSender));
            outboundReaderThread.Start();
        }

        public void Disconnect()
        {
            client.Disconnect();
            //inboundTCPReaderThread.Abort();
            inboundUDPReaderThread.Abort();
            outboundReaderThread.Abort();
        }

        private void InboundUDPReader()
        {
            try
            {
                while (true)
                {
                    GameObjectUpdate m = new GameObjectUpdate(client);
                    incomingUDPQueue.Enqueue(m);
                }
            }
            catch (Exception)
            {
                //TODO: close the client game
            }
        }
        /*
        private void InboundTCPReader()
        {
            while (this.client.IsConnected())
            {
                GameMessage m = this.client.ReadTCPMessage();
                if (m != null && m is ClientUpdate)
                {
                    incomingTCPQueue.Enqueue((ClientUpdate)m);
                }
            }
        }*/

        private void OutboundSender()
        {
            try
            {
                while (true)
                {
                    PlayerControllerUpdate m = outgoingQueue.Dequeue();
                    m.Send(this.client);
                }
            }
            catch (Exception)
            {
                //TODO: close the client game
            }
        }

        public Queue<GameObjectUpdate> DequeueAllIncomingUDP()
        {
            return incomingUDPQueue.DequeueAll();
        }

        public UdpTcpPair UdpTcpPair
        {
            get
            {
                return this.client;
            }
        }

        public int Id
        {
            get
            {
                return client.Id;
            }
        }

        public void SendUDP(PlayerControllerUpdate message)
        {
            outgoingQueue.Enqueue(message);
        }
    }
}
