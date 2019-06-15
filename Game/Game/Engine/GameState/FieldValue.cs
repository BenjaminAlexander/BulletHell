using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    public abstract class FieldValue : Serializable
    {
        public abstract int SerializationSize { get; }

        public void CopyFrom(FieldValue other)
        {
            if (this.GetType() == other.GetType())
            {
                this.Copy(other);
            }
            else
            {
                throw new Exception("Field type does not match");
            }
        }

        protected abstract void Copy(FieldValue other);

        public abstract void Serialize(byte[] buffer, ref int bufferOffset);

        public abstract void Deserialize(byte[] buffer, ref int bufferOffset);

        public abstract bool IsEqual(FieldValue other);
    }
}
