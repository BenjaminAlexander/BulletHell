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

        public static void InitializeListener()
        {
            prelimListener = new TcpListener(IPAddress.Any, LISTEN_PORT);
            prelimListener.Start();
        }

        public static void StopListener()
        {
            prelimListener.Stop();
        }

        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private Mutex tcpWriteMutex;
        private Mutex tcpReadMutex;

        private UdpClient udpClient;
        private Mutex udpWriteMutex;
        private Mutex udpReadMutex;
        private volatile bool connected;

        //TODO: fix this
        private int id = 0;
        public int Id
        {
            get
            {
                return id;
            }
        }

        //TODO: put this comment where it belongs
        //GetClientID sets up a TCP connection with the server.  
        //The server then assigns the client an integer ID.  
        //The client then closes the connection and uses the ID to 
        //set up the connection to the server.  This allows multiple 
        //clients to connect to the same server using non-colliding ports.

        //TODO: put this comment where it belongs
        //Listen, connect, and then send the new client its ID, then disconnect
        //TcpClient prelimClient = prelimListener.AcceptTcpClient();
        //(new ClientID(nextClientID)).Send(prelimClient.GetStream());
        //prelimClient.Close();

        //this constructor listens until the client is connected
        public UdpTcpPair()
        {
            this.id = nextClientID;
            nextClientID++;

            //Listen, connect, and then send the new client its ID, then disconnect
            TcpClient prelimClient = prelimListener.AcceptTcpClient();
            (new ClientID(id)).Send(prelimClient.GetStream());
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
            //Connect to the server
            TcpClient prelimTcpClient = new TcpClient();
            IPEndPoint prelimServerEndPoint = new IPEndPoint(serverIP, LISTEN_PORT);
            prelimTcpClient.Connect(prelimServerEndPoint);

            // Attempt to get the port assignment.
            ClientID portMessage = new ClientID(prelimTcpClient.GetStream());

            //close the preliminary port
            prelimTcpClient.Close();

            this.id = portMessage.ID;

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
                tcpWriteMutex.WaitOne();
                m.Send(clientStream);
                tcpWriteMutex.ReleaseMutex();
            }
        }

        public void SendUDPMessage(GameMessage m)
        {
            if (connected == true)
            {
                udpWriteMutex.WaitOne();
                m.Send(udpClient);
                udpWriteMutex.ReleaseMutex();
            }
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
            catch (Exception)
            {
                this.Disconnect();
                tcpReadMutex.ReleaseMutex();
                return null;
            }
        }

        public T ReadUDPMessage<T>() where T : GameMessage
        {
            try
            {
                T message;
                udpReadMutex.WaitOne();
                message = GameMessage.ConstructMessage<T>(this.udpClient);
                udpReadMutex.ReleaseMutex();
                return message;
            }
            catch (SocketException)
            {
                this.Disconnect();
                udpReadMutex.ReleaseMutex();
                return null;
            }
        }

        public bool IsConnected()
        {
            return connected;
        }
    }
}
