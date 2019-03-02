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

        //return the first index in buffer that is not part of the item just deserialized, i.e. the bufferOffset of the next item
        int Deserialize(byte[] buffer, int bufferOffset);

        void Serialize(byte[] buffer, int bufferOffset);
    }
}
