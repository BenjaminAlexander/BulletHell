using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using MyGame.PlayerControllers;
using MyGame.GameClient;
using MyGame.Utils;
using MyGame.GameStateObjects;

namespace MyGame.GameServer
{
    public class Player
    {
        private Lobby lobby;
        internal ControlState controller;
        private ThreadSafeQueue<PlayerControllerUpdate> incommingMessages = new ThreadSafeQueue<PlayerControllerUpdate>();
        private ThreadSafeQueue<GameObjectUpdate> outgoingUDPQueue = new ThreadSafeQueue<GameObjectUpdate>();
        private ThreadSafeQueue<TcpMessage> outgoingTCPQueue = new ThreadSafeQueue<TcpMessage>();
        private Thread outboundUDPSenderThread;
        private Thread outboundTCPSenderThread;
        private UdpTcpPair client;

        public ControlState Controller
        {
            get
            {
                return controller;
            }
        }

        public Player(Lobby lobby)
        {
            this.client = new UdpTcpPair();
            this.lobby = lobby;
            this.controller = new ControlState();

            Thread clientUDPThread = new Thread(new ThreadStart(InboundUDPClientReader));
            clientUDPThread.Start();

            /*
            Thread clientTCPThread = new Thread(new ThreadStart(InboundTCPClientReader));
            clientTCPThread.Start();
            */
            this.outboundUDPSenderThread = new Thread(new ThreadStart(OutboundUDPSender));
            this.outboundUDPSenderThread.Start();

            this.outboundTCPSenderThread = new Thread(new ThreadStart(OutboundTCPSender));
            this.outboundTCPSenderThread.Start();
        }

        private void InboundUDPClientReader()
        {
            try
            {
                while (this.client.IsConnected())
                {
                    PlayerControllerUpdate m = new PlayerControllerUpdate(this.client);
                    incommingMessages.Enqueue(m);
                }
            }
            catch (Exception) { }
            // The thread is ending, this client is done listening.
        }

        /*
        private void InboundTCPClientReader()
        {
            try
            {
                while (this.client.IsConnected())
                {
                    GameMessage m = this.client.ReadTCPMessage();

                    if (m is PlayerControllerUpdate)
                    {
                        incommingMessages.Enqueue((PlayerControllerUpdate)m);
                    }
                    else
                    {
                        throw new Exception("the client is sending messages it shouldn't");
                    }
                }
            }
            catch (Exception) { }
            // The thread is ending, this client is done listening.
        }*/

        private void OutboundUDPSender()
        {
            while (true)
            {
                GameObjectUpdate message = outgoingUDPQueue.Dequeue();
                message.Send(client);
            }
        }

        private void OutboundTCPSender()
        {
            while (true)
            {
                TcpMessage message = outgoingTCPQueue.Dequeue();
                message.Send(client);
            }
        }

        public void SendUDP(GameObjectUpdate message)
        {
            outgoingUDPQueue.Enqueue(message);
        }

        public void SendUDP(Queue<GameObjectUpdate> messages)
        {
            outgoingUDPQueue.EnqueueAll(messages);
        }

        public void SendTCP(TcpMessage message)
        {
            outgoingTCPQueue.Enqueue(message);
        }

        public void Disconnect()
        {
            client.Disconnect();
            outboundUDPSenderThread.Abort();
            outboundTCPSenderThread.Abort();
        }

        public void UpdateControlState()
        {
            Queue<PlayerControllerUpdate> messages = incommingMessages.DequeueAll();
            while (messages.Count != 0)
            {
                PlayerControllerUpdate message = messages.Dequeue();
                if (this.Id == message.PlayerID)
                {
                    message.Apply(this.controller);
                }
            }
        }

        public int Id
        {
            get
            {
                return client.Id;
            }
        }
    }
}
