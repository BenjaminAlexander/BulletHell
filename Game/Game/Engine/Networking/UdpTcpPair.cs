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
        private static int nextClientID = 1;
        private static TcpListener prelimListener;

        public static void StartListener(int preliminaryPort)
        {
            prelimListener = new TcpListener(IPAddress.Any, preliminaryPort);
            prelimListener.Start();
        }

        public static void StopListener()
        {
            prelimListener.Stop();
        }

        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private UdpClient udpClient;

        private int id;
        public int Id
        {
            get
            {
                return id;
            }
        }

        //Use this constructor for the server to listen for a client
        public UdpTcpPair(int preliminaryPort)
        {
            this.id = nextClientID;
            nextClientID++;

            //Listen, connect, and then send the new client its ID, then disconnect
            TcpClient prelimClient = prelimListener.AcceptTcpClient();

            prelimClient.GetStream().Write(BitConverter.GetBytes(id), 0, 4);
            prelimClient.GetStream().Flush();
            prelimClient.Close();

            //Start listening for that client on its port
            TcpListener tcpListener = new TcpListener(IPAddress.Any, id + preliminaryPort);
            tcpListener.Start();
            this.tcpClient = tcpListener.AcceptTcpClient();
            tcpListener.Stop();

            this.SetUp();
        }

        //Use this constructor for the client to connect to the server
        public UdpTcpPair(IPAddress serverIP, int preliminaryPort)
        {
            //Connect to the server
            TcpClient prelimTcpClient = new TcpClient();
            IPEndPoint prelimServerEndPoint = new IPEndPoint(serverIP, preliminaryPort);
            prelimTcpClient.Connect(prelimServerEndPoint);

            // Attempt to get the port assignment.
            byte[] idBuffer = new byte[4];
            prelimTcpClient.GetStream().Read(idBuffer, 0, 4);
            this.id = BitConverter.ToInt32(idBuffer, 0);

            //close the preliminary port
            prelimTcpClient.Close();

            this.tcpClient = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(serverIP, this.id + preliminaryPort);
            this.tcpClient.Connect(serverEndPoint);

            this.SetUp();
        }

        private void SetUp()
        {
            //set up UDP connection
            this.udpClient = new UdpClient((IPEndPoint)tcpClient.Client.LocalEndPoint);
            udpClient.Connect((IPEndPoint)tcpClient.Client.RemoteEndPoint);
            //Set up tcp Stream
            this.clientStream = tcpClient.GetStream();
        }

        public void Disconnect()
        {
            this.clientStream.Close();
            this.tcpClient.Close();
            this.udpClient.Close();
        }

        public void SendUDP(byte[] buffer)
        {
            udpClient.Send(buffer, buffer.Length);
        }

        public byte[] ReadUDP()
        {
            IPEndPoint ep = (IPEndPoint)udpClient.Client.RemoteEndPoint);
            return udpClient.Receive(ref ep);
        }

        public void SendTCP(byte[] buffer)
        {
            clientStream.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        public byte[] ReadTCP()
        {
            byte[] bytesToReadBuffer = new byte[sizeof(int)];
            clientStream.Read(bytesToReadBuffer, 0, sizeof(int));
            int bytesToRead = BitConverter.ToInt32(bytesToReadBuffer, 0);

            byte[] data = new byte[bytesToRead];
            clientStream.Read(data, 0, bytesToRead);
            return data;
        }
    }
}
