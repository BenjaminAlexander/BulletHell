using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.Networking;
using System.Text;
using System.Threading;

namespace EngineTest.EngineTest.NetworkingTest
{
    [TestClass]
    public class NetworkConnectionTest
    {
        public static void SetUpServerClientConnection(ThreadStart serverSideListen, ThreadStart clientSideConnect)
        {
            Thread serverListenThread = new Thread(serverSideListen);
            Thread clientConnectThread = new Thread(clientSideConnect);

            serverListenThread.Start();
            clientConnectThread.Start();
            clientConnectThread.Join();
            serverListenThread.Join();
        }

        public static void TestConnectionServerToClient(NetworkConnection serverSide, NetworkConnection clientSide, string expected)
        {
            serverSide.Send(Encoding.UTF8.GetBytes(expected));
            string actual = Encoding.UTF8.GetString(clientSide.Read());
            Assert.AreEqual(expected, actual);
        }

        public static void TestConnectionTwoWay(NetworkConnection serverSide, NetworkConnection clientSide, string expected)
        {
            TestConnectionServerToClient(serverSide, clientSide, expected + "server to client");
            TestConnectionServerToClient(clientSide, serverSide, expected + "client to server");
        }
    }
}
