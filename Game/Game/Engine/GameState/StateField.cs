using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    abstract class StateField
    {
        public StateField(StateAtInstant state)
        {
            state.AddField(this);
        }

        public abstract int Size
        {
            get;
        }

        public abstract void Deserialize(byte[] buffer, int bufferOffset);

        public abstract void Serialize(byte[] buffer, int bufferOffset);
    }
}
