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
        private volatile static int nextID;
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
        private int readBuffCurrentSize = 0;
        private int id;

        public Client(TcpClient tcpClient, UdpClient udpClient, int id)
        {
            this.id = id;
            readBuff = new byte[readBuffMaxSize];
            readBuffCurrentSize = 0;

            this.tcpClient = tcpClient;
            clientStream = tcpClient.GetStream();
            connected = true;
            tcpWriteMutex = new Mutex(false);
            tcpReadMutex = new Mutex(false);
            id = nextID;
            nextID = nextID + 1;

            this.udpClient = udpClient;
            udpWriteMutex = new Mutex(false);
            udpReadMutex = new Mutex(false);
        }

        /*public NetworkStream GetClientStream()
        {
            return clientStream;
        }*/

        public void WriteBuffer(byte[] buffer, int size)
        {
            tcpWriteMutex.WaitOne();
            try
            {
                clientStream.Write(buffer, 0, size);
                clientStream.Flush();
            }
            catch
            {
                connected = false;
                tcpClient.Close();
            } 
            tcpWriteMutex.ReleaseMutex();
        }

        public void SendTCPMessage(TCPMessage m)
        {
            if (connected == true)
            {
                if (!m.SendTCP(clientStream, tcpWriteMutex))
                {
                    connected = false;
                    tcpClient.Close();
                }
            }
        }

        public void SendUDPMessage(TCPMessage m)
        {
            if (connected == true)
            {
                if (!m.SendUDP(udpClient, tcpClient, udpWriteMutex))
                {

                }
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

        
        /*
        private void ReadToReadBuff()
        {
            readMutex.WaitOne();
            int bytesRead = Read(readBuff, readBuffCurrentSize, readBuffMaxSize - readBuffCurrentSize);
            //Console.WriteLine("Bites Read from client " + id + ": " + bytesRead);
            readBuffCurrentSize = readBuffCurrentSize + bytesRead;
            readMutex.ReleaseMutex();
        }*/
        /*
        private void ShiftReadBuff(int CurrentStart)
        {
            int i = 0;

            while (i + CurrentStart < readBuffCurrentSize)
            {
                readBuff[i] = readBuff[CurrentStart + i];
                i++;
            }

            readBuffCurrentSize = readBuffCurrentSize - CurrentStart;
        }*/

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
