using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace MyGame.Engine.Networking
{
    public class UdpTcpPair
    {
        private TCPConnection tcpConnection;
        private UDPConnection udpConnection;
        private int id;

        public int Id
        {
            get
            {
                return id;
            }
        }

        //Use this constructor for the server to listen for a client
        public UdpTcpPair(TcpListener listener, int id)
        {
            this.id = id;

            //Listen, connect, and then send the new client its ID, then disconnect
            this.tcpConnection = TCPConnection.Listen(listener);
            this.tcpConnection.Send(BitConverter.GetBytes(id));
            this.udpConnection = new UDPConnection(this.tcpConnection.LocalEndpoint, this.tcpConnection.RemoteEndpoint);
        }

        //Use this constructor for the client to connect to the server
        public UdpTcpPair(IPAddress serverIP, int port)
        {
            //Connect to the server
            this.tcpConnection = TCPConnection.Connect(serverIP, port);

            // Attempt to get the id
            byte[] idBuffer = this.tcpConnection.Read();
            this.id = BitConverter.ToInt32(idBuffer, 0);

            this.udpConnection = new UDPConnection(this.tcpConnection.LocalEndpoint, this.tcpConnection.RemoteEndpoint);
        }

        public void Close()
        {
            this.tcpConnection.Close();
            this.udpConnection.Close();
        }

        public void SendUDP(byte[] buffer)
        {
            this.udpConnection.Send(buffer);
        }

        public void SendUDP(byte[] buffer, int length)
        {
            this.udpConnection.Send(buffer, length);
        }

        public byte[] ReadUDP()
        {
            return this.udpConnection.Read();
        }

        public void SendTCP(byte[] buffer)
        {
            this.tcpConnection.Send(buffer);
        }

        public void SendTCP(byte[] buffer, int length)
        {
            this.tcpConnection.Send(buffer, length);
        }

        public byte[] ReadTCP()
        {
            return this.tcpConnection.Read();
        }
    }
}
