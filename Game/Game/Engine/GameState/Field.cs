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

            public void CopyFrom(Field other, int instant)
            {
                if(this.GetType() == other.GetType())
                {
                    this.CopyFrom(other, instant);
                }
                else
                {
                    throw new Exception("Field type does not match");
                }
            }

            protected abstract void Copy(Field other, int instant);

            public abstract int Size
            {
                get;
            }

            public abstract void Deserialize(int instant, byte[] buffer, ref int bufferOffset);

            public abstract void Serialize(int instant, byte[] buffer, int bufferOffset);
        }
    }
}
