using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState.ObjectState
{
    partial class ObjectState
    {
        public abstract class Field : Serializable
        {
            public Field(ObjectState obj)
            {
                obj.AddField(this);
            }

            public abstract int SerializationSize
            {
                get;
            }

            public abstract int Deserialize(byte[] buffer, int bufferOffset);

            public abstract void Serialize(byte[] buffer, int bufferOffset);
        }
    }
}
