using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Networking
{
    public interface NetworkConnection
    {
        void Send(byte[] buffer);
        void Send(byte[] buffer, int length);
        byte[] Read();
        void Close();
    }
}
