using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    static class Utils
    {
        public static T Deserialize<T>(byte[] buffer, ref int bufferOffset) where T : Deserializable, new()
        {
            T obj = new T();
            obj.Deserialize(buffer, ref bufferOffset);
            return obj;
        }

        public static T Deserialize<T>(byte[] buffer) where T : Deserializable, new()
        {
            int offset = 0;
            return Deserialize<T>(buffer, ref offset);
        }

        public static int ReadInt(byte[] buffer, ref int bufferOffset)
        {
            int value = BitConverter.ToInt32(buffer, bufferOffset);
            bufferOffset = bufferOffset + sizeof(int);
            return value;
        }

        public static float ReadFloat(byte[] buffer, ref int bufferOffset)
        {
            float value = BitConverter.ToSingle(buffer, bufferOffset);
            bufferOffset = bufferOffset + sizeof(float);
            return value;
        }

        public static void Deserialize(Deserializable obj, byte[] buffer)
        {
            int offset = 0;
            obj.Deserialize(buffer, ref offset);
        }
    }
}
