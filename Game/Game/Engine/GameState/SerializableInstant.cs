using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    interface SerializableInstant
    {
        int SerializationSize
        {
            get;
        }

        void Serialize(Instant instant, byte[] buffer, int bufferOffset);

        void Deserialize(Instant instant, byte[] buffer, int bufferOffset);
    }
}
