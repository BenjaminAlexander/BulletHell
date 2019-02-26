using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.Networking;
using System.Net;
using System.Threading;

namespace EngineTest.EngineTest.NetworkingTest
{
    [TestClass]
    public class UDPConnectionTest
    {
        private UDPConnection serverSide;
        private UDPConnection clientSide;

        private void ServerSideListen()
        {
            serverSide = new UDPConnection(IPAddress.Parse("127.0.0.1"), 3000, 3001);
        }

        private void ClientSideConnect()
        {
            clientSide = new UDPConnection(IPAddress.Parse("127.0.0.1"), 3001, 3000);
        }

        [TestMethod]
        public void TwoWayUDPConnectionTest()
        {
            NetworkConnectionTest.SetUpServerClientConnection(new ThreadStart(ServerSideListen), new ThreadStart(ClientSideConnect));
            NetworkConnectionTest.TestConnectionTwoWay(serverSide, clientSide, "UDP Connection Test");
            serverSide.Close();
            clientSide.Close();
        }
    }
}
