using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface Serializable
    {
        int SerializationSize
        {
            get;
        }

        void Serialize(byte[] buffer, ref int bufferOffset);
    }
}
