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

        static int Main(string[] args)
        {
            //automatically fail test to test go cd
            return 1;

            IPAddress address = IPAddress.Parse("127.0.0.1");
            if (args.Length > 1 && !IPAddress.TryParse(args[0], out address))
            {
                Console.WriteLine("Cannot parse server IP address");
                return 1;
            }

            TcpClient prelimTcpClient = new TcpClient();
            IPEndPoint prelimServerEndPoint = new IPEndPoint(address, LISTEN_PORT);
            prelimTcpClient.Connect(prelimServerEndPoint);
            if (prelimTcpClient.Connected)
            {
                Console.WriteLine("Connected to Metaserver at " + address.ToString());
                prelimTcpClient.Close();
                return 0;
            }
            else
            {
                Console.WriteLine("Could not connect to Metaserver at " + address.ToString());
                return 1;
            }
        }
    }
}
