using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MetaserverTest
{
    class Program
    {
        private const int LISTEN_PORT = 3001;

        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            TcpClient prelimTcpClient = new TcpClient();
            IPEndPoint prelimServerEndPoint = new IPEndPoint(address, LISTEN_PORT);
            prelimTcpClient.Connect(prelimServerEndPoint);
            prelimTcpClient.Close();
        }
    }
}
