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
            private GameObject owner;

            protected InstantSelector InstantSelector
            {
                get
                {
                    return owner.instantSelector;
                }
            }

            public Field(GameObject obj)
            {
                obj.AddField(this);
                this.owner = obj;
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

            public abstract bool FieldAtInstantExists(int instant);

            public abstract void CopyInstant(int from, int to);

            protected abstract void Copy(Field other, int instant);

            public abstract int SerializationSize(int instant);

            public abstract void Deserialize(int instant, byte[] buffer, ref int bufferOffset);

            public abstract void Serialize(int instant, byte[] buffer, ref int bufferOffset);

            public abstract bool Remove(int instant);
        }
    }
}
