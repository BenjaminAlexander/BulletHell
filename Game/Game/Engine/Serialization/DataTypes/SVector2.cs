using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MyGame.Engine.Serialization.DataTypes
{
    struct SVector2 : Serializable, Deserializable
    {
        Vector2 value;

        public SVector2(Vector2 value)
        {
            this.value = value;
        }

        public Vector2 Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }


        public static implicit operator Vector2(SVector2 s)
        {
            return s.value;
        }

        public static implicit operator SVector2(Vector2 value)
        {
            return new SVector2(value);
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(float) * 2;
            }
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(this.Value.X, buffer, ref bufferOffset);
            Utils.Write(this.Value.Y, buffer, ref bufferOffset);
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            float x;
            float y;
            Utils.Read(out x, buffer, ref bufferOffset);
            Utils.Read(out y, buffer, ref bufferOffset);
            this.value = new Vector2(x, y);
        }
    }
}
