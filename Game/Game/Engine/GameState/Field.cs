using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    partial class GameObject
    {
        public abstract class Field
        {
            public Field(GameObject obj)
            {
                obj.AddField(this);
            }

            public abstract int Size
            {
                get;
            }

            public abstract void Deserialize(Instant instant, byte[] buffer, int bufferOffset);

            public abstract void Serialize(Instant instant, byte[] buffer, int bufferOffset);
        }
    }
}
