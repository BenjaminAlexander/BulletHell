using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.Networking;
using System.Net;
using System.Threading;

namespace EngineTest.EngineTest.NetworkingTest
{
    [TestClass]
    public class TCPConnectionTest
    {
        private TCPConnection serverSide;
        private TCPConnection clientSide;

        private void ServerSideListen()
        {
            serverSide = TCPConnection.Listen(IPAddress.Parse("127.0.0.1"), 3000);
        }

        private void ClientSideConnect()
        {
            clientSide = TCPConnection.Connect(IPAddress.Parse("127.0.0.1"), 3000);
        }

        [TestMethod]
        public void TwoWayTCPConnectionTest()
        {
            NetworkConnectionTest.SetUpServerClientConnection(new ThreadStart(ServerSideListen), new ThreadStart(ClientSideConnect));
            NetworkConnectionTest.TestConnectionTwoWay(serverSide, clientSide, "TCP Connection Test");
            serverSide.Close();
            clientSide.Close();
        }
    }
}
