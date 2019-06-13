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
        public abstract class Field : Serializable
        {
            private GameObject owner;

            public abstract int SerializationSize { get; }

            public Field(GameObject obj)
            {
                obj.AddField(this);
                this.owner = obj;
            }

            public void CopyFrom(Field other)
            {
                if(this.GetType() == other.GetType())
                {
                    this.Copy(other);
                }
                else
                {
                    throw new Exception("Field type does not match");
                }
            }

            protected abstract void Copy(Field other);
      
            public abstract void SetWriteField(Field writeField);

            public abstract void Serialize(byte[] buffer, ref int bufferOffset);

            public abstract void Deserialize(byte[] buffer, ref int bufferOffset);
        }
    }
}
