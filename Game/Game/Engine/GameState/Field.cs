using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    partial class GameObject
    {
        public abstract class Field
        {
            private InstantSelector instantSelector;

            public Field(GameObject obj)
            {
                obj.AddField(this);
                this.instantSelector = obj.InstantSelector;
            }

            public InstantSelector InstantSelector
            {
                get
                {
                    return instantSelector;
                }

                set
                {
                    instantSelector = value;
                }
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

            public abstract void InitializeNextInstant(int currentInstant);

            public abstract void InitializeInstant(int instant);

            protected abstract void Copy(Field other, int instant);

            public abstract int SerializationSize(int instant);

            public abstract void Deserialize(int instant, byte[] buffer, ref int bufferOffset);

            public abstract void Serialize(int instant, byte[] buffer, ref int bufferOffset);
        }
    }
}
