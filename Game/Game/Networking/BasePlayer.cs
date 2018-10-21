﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using MyGame.Utils;
using System.Threading;
using System.Net;
using MyGame.GameClient;

namespace MyGame.Networking
{
    public abstract class BasePlayer<InUDP> where InUDP : UdpMessage
    {
        public UdpTcpPair client;

        private ThreadSafeQueue<UdpMessage> outgoingUDPQueue = new ThreadSafeQueue<UdpMessage>();
        private ThreadSafeQueue<InUDP> incomingUDPQueue = new ThreadSafeQueue<InUDP>();

        private ThreadSafeQueue<TcpMessage> outgoingTCPQueue = new ThreadSafeQueue<TcpMessage>();

        private Thread outboundUDPSenderThread;
        private Thread inboundUDPReaderThread;

        private Thread outboundTCPSenderThread;

        public BasePlayer(UdpTcpPair udpTcpPair)
        {
            this.client = udpTcpPair;
            this.StartThreads();
        }

        private void StartThreads()
        {
            this.inboundUDPReaderThread = new Thread(new ThreadStart(InboundUDPReader));
            this.inboundUDPReaderThread.Start();

            this.outboundUDPSenderThread = new Thread(new ThreadStart(OutboundUDPSender));
            this.outboundUDPSenderThread.Start();

            this.outboundTCPSenderThread = new Thread(new ThreadStart(OutboundTCPSender));
            this.outboundTCPSenderThread.Start();
        }

        public virtual void Disconnect()
        {
            outboundUDPSenderThread.Abort();
            inboundUDPReaderThread.Abort();
            outboundTCPSenderThread.Abort();
            client.Disconnect();
        }

        public void SendUDP(UdpMessage message)
        {
            outgoingUDPQueue.Enqueue(message);
        }

        public Queue<InUDP> DequeueAllIncomingUDP()
        {
            return incomingUDPQueue.DequeueAll();
        }

        private void OutboundUDPSender()
        {
            try
            {
                while (true)
                {
                    UdpMessage m = outgoingUDPQueue.Dequeue();
                    m.Send(this.client);
                }
            }
            catch (Exception)
            {
                //TODO: notify the caller somehow
            }
        }

        private void InboundUDPReader()
        {
            try
            {
                while (true)
                {
                    InUDP m = this.GetUDPMessage(this.client);
                    incomingUDPQueue.Enqueue(m);
                }
            }
            catch (Exception)
            {
                //TODO: close the client game
            }
        }

        public void SendTCP(TcpMessage message)
        {
            outgoingTCPQueue.Enqueue(message);
        }

        private void OutboundTCPSender()
        {
            try
            {
                while (true)
                {
                    TcpMessage message = outgoingTCPQueue.Dequeue();
                    message.Send(client);
                }
            }
            catch (Exception)
            {
                //TODO: close the client game
            }
        }

        public int Id
        {
            get
            {
                return client.Id;
            }
        }

        public abstract InUDP GetUDPMessage(UdpTcpPair client);

        public SetWorldSize GetSetWorldSize()
        {
            return new SetWorldSize(this.client);
        }
    }
}
