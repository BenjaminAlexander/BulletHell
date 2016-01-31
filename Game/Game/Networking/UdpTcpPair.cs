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
        private const int LISTEN_PORT = 3000;
        private static int nextClientID = 1;
        private static TcpListener prelimListener;

        public static void StartListener()
        {
            GameMessage.Initialize();
            prelimListener = new TcpListener(IPAddress.Any, LISTEN_PORT);
            prelimListener.Start();
        }

        public static void StopListener()
        {
            prelimListener.Stop();
        }

        private TcpClient tcpClient;
        private NetworkStream clientStream;

        private UdpClient udpClient;

        //TODO: fix this
        private int id;
        public int Id
        {
            get
            {
                return id;
            }
        }

        //this constructor listens until the client is connected
        public UdpTcpPair()
        {
            this.id = nextClientID;
            nextClientID++;

            //Listen, connect, and then send the new client its ID, then disconnect
            TcpClient prelimClient = prelimListener.AcceptTcpClient();

            prelimClient.GetStream().Write(BitConverter.GetBytes(id), 0, 4);
            prelimClient.GetStream().Flush();
            prelimClient.Close();

            //Start listening for that client on its port
            TcpListener tcpListener = new TcpListener(IPAddress.Any, id + LISTEN_PORT);
            tcpListener.Start();
            this.tcpClient = tcpListener.AcceptTcpClient();
            tcpListener.Stop();

            this.SetUp();
        }

        //this constructor blocks until the client is connected
        public UdpTcpPair(IPAddress serverIP)
        {
            GameMessage.Initialize();

            //Connect to the server
            TcpClient prelimTcpClient = new TcpClient();
            IPEndPoint prelimServerEndPoint = new IPEndPoint(serverIP, LISTEN_PORT);
            prelimTcpClient.Connect(prelimServerEndPoint);

            // Attempt to get the port assignment.
            byte[] idBuffer = new byte[4];
            prelimTcpClient.GetStream().Read(idBuffer, 0, 4);
            this.id = BitConverter.ToInt32(idBuffer, 0);

            //close the preliminary port
            prelimTcpClient.Close();

            //this.id = portMessage.ID;

            this.tcpClient = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(serverIP, this.id + LISTEN_PORT);
            this.tcpClient.Connect(serverEndPoint);

            this.SetUp();
        }

        private void SetUp()
        {
            //set up UDP connection
            this.udpClient = new UdpClient((IPEndPoint)tcpClient.Client.LocalEndPoint);
            udpClient.Connect((IPEndPoint)tcpClient.Client.RemoteEndPoint);

            this.clientStream = tcpClient.GetStream();
        }

        public void Disconnect()
        {
            this.clientStream.Close();
            this.tcpClient.Close();
            this.udpClient.Close();
        }

        internal NetworkStream ClientStream
        {
            get
            {
                return clientStream;
            }
        }

        internal UdpClient UdpClient
        {
            get
            {
                return udpClient;
            }
        }
    }
}
