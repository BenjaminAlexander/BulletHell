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

        private static ThreadSafeQueue<PlayerControllerUpdate> outgoingUDPQueue = new ThreadSafeQueue<PlayerControllerUpdate>();
        private static ThreadSafeQueue<GameObjectUpdate> incomingUDPQueue = new ThreadSafeQueue<GameObjectUpdate>();

        private Thread inboundUDPReaderThread;
        private Thread outboundReaderThread;

        public ServerConnection(IPAddress serverAddress)
        {
            this.client = new UdpTcpPair(serverAddress);

            inboundUDPReaderThread = new Thread(new ThreadStart(InboundUDPReader));
            inboundUDPReaderThread.Start();

            outboundReaderThread = new Thread(new ThreadStart(OutboundSender));
            outboundReaderThread.Start();
        }

        public void Disconnect()
        {
            client.Disconnect();
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

        private void OutboundSender()
        {
            try
            {
                while (true)
                {
                    PlayerControllerUpdate m = outgoingUDPQueue.Dequeue();
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
            outgoingUDPQueue.Enqueue(message);
        }
    }
}
