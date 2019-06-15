using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Networking
{
    public class TCPConnection : NetworkConnection
    {
        private TcpClient tcpClient;
        private NetworkStream clientStream;

        public IPEndPoint LocalEndpoint
        {
            get
            {
                return (IPEndPoint)tcpClient.Client.LocalEndPoint;
            }
        }

        public IPEndPoint RemoteEndpoint
        {
            get
            {
                return (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            }
        }

        public static TCPConnection Listen(IPAddress ipAddress, int port)
        {
            TcpListener tcpListener = new TcpListener(ipAddress, port);
            tcpListener.Start();
            TCPConnection newConnection = TCPConnection.Listen(tcpListener);
            tcpListener.Stop();
            return newConnection;
        }

        public static TCPConnection Listen(TcpListener tcpListener)
        {
            TCPConnection newConnection = new TCPConnection(tcpListener.AcceptTcpClient());
            return newConnection;
        }

        public static TCPConnection Connect(IPAddress ipAddress, int port)
        {
            TcpClient tcpClient = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, port);
            tcpClient.Connect(serverEndPoint);
            return new TCPConnection(tcpClient);
        }

        private TCPConnection(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.clientStream = tcpClient.GetStream();
        }

        public byte[] Read()
        {
            byte[] bytesToReadBuffer = new byte[sizeof(int)];
            clientStream.Read(bytesToReadBuffer, 0, sizeof(int));
            int bytesToRead = BitConverter.ToInt32(bytesToReadBuffer, 0);

            byte[] data = new byte[bytesToRead];
            clientStream.Read(data, 0, bytesToRead);
            return data;
        }

        public void Send(byte[] buffer)
        {
            clientStream.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        public void Send(byte[] buffer, int length)
        {
            clientStream.Write(BitConverter.GetBytes(length), 0, sizeof(int));
            clientStream.Write(buffer, 0, length);
            clientStream.Flush();
        }

        public void Close()
        {
            this.clientStream.Close();
            this.tcpClient.Close();
        }
    }
}
