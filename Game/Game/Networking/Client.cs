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


        public int ReadTCP(byte[] buffer, int offset, int size)
        {
            tcpReadMutex.WaitOne();
            //blocks until a client sends a message
            int bytesRead = 0;

            while (bytesRead != size)
            {
                try
                {
                    bytesRead = clientStream.Read(buffer, offset + bytesRead, size - bytesRead);
                }
                catch
                {
                    connected = false;
                    tcpClient.Close();
                }

                if (bytesRead == 0)
                {
                    connected = false;
                    tcpClient.Close();
                }

                if (bytesRead > size)
                {
                    throw new Exception("read error");
                }
            }

            tcpReadMutex.ReleaseMutex();
            return bytesRead;
        }

        public byte[] ReadUDP()
        {
            udpReadMutex.WaitOne();
            IPEndPoint ep = (IPEndPoint)udpClient.Client.RemoteEndPoint;
            byte[] mBuff = udpClient.Receive(ref ep);

            udpReadMutex.ReleaseMutex();
            return mBuff;
        }

        public bool IsConnected()
        {
            return connected;
        }

        public int GetID()
        {
            return id;
        }
         
    }
}
