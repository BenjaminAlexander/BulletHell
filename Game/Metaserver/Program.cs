using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Sockets;
using System.Net;

namespace Metaserver
{
    class Program
    {
        private const int LISTEN_PORT = 3001;
        private static TcpListener prelimListener;

        static void Main(string[] args)
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine("BulletHell metaserver is up Version: " + version);
            prelimListener = new TcpListener(IPAddress.Any, LISTEN_PORT);
            prelimListener.Start();
            while (true)
            {
                TcpClient prelimClient = prelimListener.AcceptTcpClient();
                Console.WriteLine(prelimClient.Client.RemoteEndPoint.ToString());
                prelimClient.Close();
            }
            Console.WriteLine("BulletHell metaserver is down");
        }
    }
}
