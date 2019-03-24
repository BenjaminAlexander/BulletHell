using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    static class Utils
    {
        public static T Deserialize<T>(InstantTypeSerializer<T> serializer, byte[] buffer, ref int bufferOffset) where T : InstantSerializable
        {
            T obj = serializer.Deserialize(buffer, ref bufferOffset);
            return obj;
        }

        public static T Deserialize<T>(InstantTypeSerializer<T> serializer, byte[] buffer) where T : InstantSerializable
        {
            int offset = 0;
            return Deserialize<T>(serializer, buffer, ref offset);
        }

        public static T Deserialize<T>(byte[] buffer, ref int bufferOffset) where T : Serializable, new()
        {
            T obj = new T();
            obj.Deserialize(buffer, ref bufferOffset);
            return obj;
        }

        public static T Deserialize<T>(byte[] buffer) where T : Serializable, new()
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

        public static bool ReadBool(byte[] buffer, ref int bufferOffset)
        {
            bool value = BitConverter.ToBoolean(buffer, bufferOffset);
            bufferOffset = bufferOffset + sizeof(bool);
            return value;
        }

        public static void Write(int value, byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, bufferOffset, sizeof(int));
            bufferOffset = bufferOffset + sizeof(int);
        }

        public static void Write(float value, byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, bufferOffset, sizeof(float));
            bufferOffset = bufferOffset + sizeof(float);
        }

        public static void Write(bool value, byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, bufferOffset, sizeof(bool));
            bufferOffset = bufferOffset + sizeof(bool);
        }

        public static void Deserialize(Serializable obj, byte[] buffer)
        {
            int offset = 0;
            obj.Deserialize(buffer, ref offset);
        }
    }
}
