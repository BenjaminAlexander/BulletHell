using Microsoft.Xna.Framework;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.FieldValues
{
    public struct Vector2Value : FieldValue
    {
        private Vector2 value;

        public Vector2Value(Vector2 value)
        {
            this.value = value;
        }

        public Vector2 Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public static implicit operator Vector2Value(Vector2 value)
        {
            return new Vector2Value(value);
        }

        public static implicit operator Vector2(Vector2Value vector2Value)
        {
            return vector2Value.value;
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(float) * 2;
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            float x = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
            float y = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
            this.Value = new Vector2(x, y);
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(this.Value.X, buffer, ref bufferOffset);
            Serialization.Utils.Write(this.Value.Y, buffer, ref bufferOffset);
        }

        public override bool Equals(object obj)
        {
            if(obj is Vector2Value)
            {
                return this.value.Equals(((Vector2Value)obj).value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}
