using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    abstract class StateMetaField
    {
        public StateMetaField(GameObject obj)
        {
            obj.AddField(this);
        }

        public abstract int Size
        {
            get;
        }

        public abstract void Deserialize(int instant, byte[] buffer, int bufferOffset);

        public abstract void Serialize(int instant, byte[] buffer, int bufferOffset);
    }
}
