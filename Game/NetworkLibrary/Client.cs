using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;


namespace NetworkingLibrary
{
    public class Client
    {
        private volatile static int nextID;
        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private Mutex writeMutex;
        private Mutex readMutex;
        private volatile bool connected;
        private volatile int id;

        private byte[] readBuff;
        private const int readBuffMaxSize = 1024;
        private int readBuffCurrentSize = 0;


        public Client(TcpClient tcpClient)
        {
            readBuff = new byte[readBuffMaxSize];
            readBuffCurrentSize = 0;

            this.tcpClient = tcpClient;
            clientStream = tcpClient.GetStream();
            connected = true;
            writeMutex = new Mutex(false);
            readMutex = new Mutex(false);
            id = nextID;
            nextID = nextID + 1;
        }

        public NetworkStream GetClientStream()
        {
            return clientStream;
        }

        public void WriteBuffer(byte[] buffer, int offset, int size)
        {
            writeMutex.WaitOne();
            clientStream.Write(buffer, 0, size);
            clientStream.Flush();
            writeMutex.ReleaseMutex();
        }


        private int Read(byte[] buffer, int offset, int size)
        {
            
            //blocks until a client sends a message
            int bytesRead = 0;
            try
            {
                bytesRead = clientStream.Read(buffer, offset, size);
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

            return bytesRead;
        }

        private void ReadToReadBuff()
        {
            readMutex.WaitOne();
            int bytesRead = Read(readBuff, readBuffCurrentSize, readBuffMaxSize - readBuffCurrentSize);
            //Console.WriteLine("Bites Read from client " + id + ": " + bytesRead);
            readBuffCurrentSize = readBuffCurrentSize + bytesRead;
            readMutex.ReleaseMutex();
        }

        private void ShiftReadBuff(int CurrentStart)
        {
            int i = 0;

            while (i + CurrentStart < readBuffCurrentSize)
            {
                readBuff[i] = readBuff[CurrentStart + i];
                i++;
            }

            readBuffCurrentSize = readBuffCurrentSize - CurrentStart;
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
