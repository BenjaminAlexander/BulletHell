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

        void Deserialize(byte[] buffer, int bufferOffset);
        void Serialize(byte[] buffer, int bufferOffset);
    }
}
