using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace MyGame.Networking
{
    public static class NetUtils
    {
        public static string GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        public static int ReadTCP(NetworkStream clientStream, byte[] buffer, int offset, int size)
        {
            //blocks until a client sends a message
            int totalBytesRead = 0;

            // This will block until the whole message is read, or until something goes wrong.
            while (totalBytesRead != size)
            {
                if (clientStream.CanRead)
                {
                    totalBytesRead += clientStream.Read(buffer, offset + totalBytesRead, size - totalBytesRead);
                }
                else
                {
                    // If we cannot read, throwing an exception.
                    throw new Client.ClientNotConnectedException();
                }
            }
            return totalBytesRead;
        }

        public static GameMessage ReadTCPMessage(NetworkStream networkStream)
        {
            var readBuff = new byte[GameMessage.BUFF_MAX_SIZE];
            int amountRead = NetUtils.ReadTCP(networkStream, readBuff, 0, GameMessage.HEADER_SIZE);
            int bytesLeft = BitConverter.ToInt32(readBuff, GameMessage.LENGTH_POSITION);
            amountRead = amountRead + NetUtils.ReadTCP(networkStream, readBuff, amountRead, bytesLeft);
            return GameMessage.ConstructMessage(readBuff, amountRead);
        }
    }
}
