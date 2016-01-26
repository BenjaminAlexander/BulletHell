using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;


namespace MyGame.Networking
{
    public class UdpTcpPair
    {
        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private Mutex tcpWriteMutex;
        private Mutex tcpReadMutex;

        private UdpClient udpClient;
        private Mutex udpWriteMutex;
        private Mutex udpReadMutex;
        private volatile bool connected;

        //this constructor listens until the client is connected
        public UdpTcpPair(int port)
        {
            //Start listening for that client on its port
            TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            this.tcpClient = tcpListener.AcceptTcpClient();
            tcpListener.Stop();

            this.SetUp();
        }

        //this constructor blocks until the client is connected
        public UdpTcpPair(IPAddress address, int port)
        {
            this.tcpClient = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(address, port);
            this.tcpClient.Connect(serverEndPoint);

            this.SetUp();
        }

        private void SetUp()
        {
            //set up UDP connection
            this.udpClient = new UdpClient((IPEndPoint)tcpClient.Client.LocalEndPoint);
            udpClient.Connect((IPEndPoint)tcpClient.Client.RemoteEndPoint);

            this.clientStream = tcpClient.GetStream();
            this.connected = true;
            this.tcpWriteMutex = new Mutex(false);
            this.tcpReadMutex = new Mutex(false);

            this.udpWriteMutex = new Mutex(false);
            this.udpReadMutex = new Mutex(false);
        }

        public void Disconnect()
        {
            this.connected = false;
            this.clientStream.Close();
            this.tcpClient.Close();
            this.udpClient.Close();
        }

        public void SendTCPMessage(GameMessage m)
        {
            if (connected == true)
            {
                //m.ClientID = this.id;
                tcpWriteMutex.WaitOne();
                m.SendTCP(clientStream);
                tcpWriteMutex.ReleaseMutex();
            }
        }

        public void SendUDPMessage(GameMessage m)
        {
            if (connected == true)
            {
                //m.ClientID = this.id;
                udpWriteMutex.WaitOne();
                m.SendUDP(udpClient);
                udpWriteMutex.ReleaseMutex();
            }
        }

        private byte[] ReadUDP()
        {
            udpReadMutex.WaitOne();
            IPEndPoint ep = (IPEndPoint)udpClient.Client.RemoteEndPoint;

            byte[] mBuff;
            try
            {
                mBuff = udpClient.Receive(ref ep);
            }
            catch (SocketException)
            {
                // If we cannot read from the udp client, we've been disconnected.  Throw an exception to inform the caller.
                this.Disconnect();
                udpReadMutex.ReleaseMutex();
                throw new ClientNotConnectedException();
            }
            udpReadMutex.ReleaseMutex();
            return mBuff;
        }

        public GameMessage ReadTCPMessage()
        {
            try
            {
                tcpReadMutex.WaitOne();
                GameMessage message = GameMessage.ConstructMessage(this.clientStream);
                tcpReadMutex.ReleaseMutex();
                return message;
            }
            catch (ClientNotConnectedException e)
            {
                this.Disconnect();
                tcpReadMutex.ReleaseMutex();
                throw e;
            }
        }

        public GameMessage ReadUDPMessage()
        {
            byte[] readBuff;
            try
            {
                readBuff = this.ReadUDP();
            }
            catch (ClientNotConnectedException e)
            {
                throw e;
            }
            return GameMessage.ConstructMessage(readBuff, readBuff.Length);
        }

        public bool IsConnected()
        {
            return connected;
        }


        public class ClientNotConnectedException : Exception { }
    }
}
