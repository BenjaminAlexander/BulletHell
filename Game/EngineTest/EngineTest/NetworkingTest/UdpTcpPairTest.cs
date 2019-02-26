using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.Networking;
using System.Net;
using System.Threading;
using System.Text;
using System.Net.Sockets;

namespace EngineTest.EngineTest.NetworkingTest
{
    [TestClass]
    public class UdpTcpPairTest
    {
        public UdpTcpPair serverSide;
        public UdpTcpPair clientSide;

        private void ServerSideListen()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 3000);
            listener.Start();
            serverSide = new UdpTcpPair(listener, 1);
            listener.Stop();
        }

        private void ClientSideConnect()
        {
            clientSide = new UdpTcpPair(IPAddress.Parse("127.0.0.1"), 3000);
        }

        [TestMethod]
        public void SendBufferTest()
        {
            NetworkConnectionTest.SetUpServerClientConnection(new ThreadStart(ServerSideListen), new ThreadStart(ClientSideConnect));
            string expected;
            string actual;

            expected = "Server to client TCP";
            serverSide.SendTCP(Encoding.UTF8.GetBytes(expected));
            actual = Encoding.UTF8.GetString(clientSide.ReadTCP());
            Assert.AreEqual(expected, actual);

            expected = "Client to server TCP";
            clientSide.SendTCP(Encoding.UTF8.GetBytes(expected));
            actual = Encoding.UTF8.GetString(serverSide.ReadTCP());
            Assert.AreEqual(expected, actual);

            expected = "Server to client UDP";
            serverSide.SendUDP(Encoding.UTF8.GetBytes(expected));
            actual = Encoding.UTF8.GetString(clientSide.ReadUDP());
            Assert.AreEqual(expected, actual);

            expected = "Client to server UDP";
            clientSide.SendUDP(Encoding.UTF8.GetBytes(expected));
            actual = Encoding.UTF8.GetString(serverSide.ReadUDP());
            Assert.AreEqual(expected, actual);

            serverSide.Close();
            clientSide.Close();
        }
    }
}
