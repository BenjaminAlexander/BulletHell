using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace MyGame.Networking
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private Mutex tcpWriteMutex;
        private Mutex tcpReadMutex;
        private UdpClient udpClient;
        private Mutex udpWriteMutex;
        private Mutex udpReadMutex;
        private volatile bool connected;

        private byte[] readBuff;
        private const int readBuffMaxSize = 1024;
        private int id;

        public Client(TcpClient tcpClient, UdpClient udpClient, int id)
        {
            this.id = id;
            readBuff = new byte[readBuffMaxSize];

            this.tcpClient = tcpClient;
            clientStream = tcpClient.GetStream();
            connected = true;
            tcpWriteMutex = new Mutex(false);
            tcpReadMutex = new Mutex(false);

            this.udpClient = udpClient;
            udpWriteMutex = new Mutex(false);
            udpReadMutex = new Mutex(false);
        }

        public void SendTCPMessage(GameMessage m)
        {
            if (connected == true)
            {
                m.ClientID = this.id;
                m.SendTCP(clientStream, tcpWriteMutex);
            }
        }

        public void SendUDPMessage(GameMessage m)
        {
            if (connected == true)
            {
                m.ClientID = this.id;
                m.SendUDP(udpClient, tcpClient, udpWriteMutex);
            }
        }

        private int ReadTCP(byte[] buffer, int offset, int size)
        {
            tcpReadMutex.WaitOne();
            //blocks until a client sends a message
            int totalBytesRead = 0;

            // TODO: Warning: This will block until the whole message is read, or until something goes wrong.
            while (totalBytesRead != size)
            {
                if (clientStream.CanRead)
                {
                    totalBytesRead += clientStream.Read(buffer, offset + totalBytesRead, size - totalBytesRead);
                }
                else
                {
                    // If we cannot read, shut everything down and mark this client as diconnected, throwing an exception.
                    clientStream.Close();
                    tcpReadMutex.ReleaseMutex();
                    connected = false;
                    throw new ClientNotConnectedException();
                }
            }

            tcpReadMutex.ReleaseMutex();
            return totalBytesRead;
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
                connected = false;
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
                var readBuff = new byte[GameMessage.BUFF_MAX_SIZE];
                int amountRead = ReadTCP(readBuff, 0, GameMessage.HEADER_SIZE);
                int bytesLeft = BitConverter.ToInt32(readBuff, GameMessage.LENGTH_POSITION);
                amountRead = amountRead + ReadTCP(readBuff, amountRead, bytesLeft);
                return GameMessage.ConstructMessage(readBuff, amountRead);
            }
            catch (ClientNotConnectedException e)
            {
                throw e;
            }
        }

        public GameMessage ReadUDPMessage()
        {
            byte[] readBuff;
            try
            {
                 readBuff = ReadUDP();
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

        public int GetID()
        {
            return id;
        }

        public class ClientNotConnectedException : Exception { }
    }
}
