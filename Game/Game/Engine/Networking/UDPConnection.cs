using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace MyGame.Engine.Networking
{
    class UDPConnection : NetworkConnection
    {
        private UdpClient udpClient;

        public UDPConnection(IPAddress ipAddress, int localPort, int remotePort)
        {
            this.udpClient = new UdpClient(localPort);
            this.udpClient.Connect(ipAddress, remotePort);
        }

        public UDPConnection(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint)
        {
            this.udpClient = new UdpClient(localEndpoint);
            this.udpClient.Connect(remoteEndpoint);
        }

        public void Send(byte[] buffer)
        {
            udpClient.Send(buffer, buffer.Length);
        }

        public void Send(byte[] buffer, int length)
        {
            udpClient.Send(buffer, length);
        }

        public byte[] Read()
        {
            IPEndPoint ep = (IPEndPoint)udpClient.Client.RemoteEndPoint;
            return udpClient.Receive(ref ep);
        }

        public void Close()
        {
            this.udpClient.Close();
        }
    }
}
