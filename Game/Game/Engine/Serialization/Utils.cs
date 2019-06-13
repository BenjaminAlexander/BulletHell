using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    static class Utils
    {
        public static void Deserialize(Deserializable obj, byte[] buffer)
        {
            int offset = 0;
            obj.Deserialize(buffer, ref offset);
        }

        public static byte[] Serialize(Serializable obj)
        {
            byte[] buffer = new byte[obj.SerializationSize];
            int bufferOffset = 0;
            obj.Serialize(buffer, ref bufferOffset);
            return buffer;
        }

        public static byte[] Serialize(InstantSerializable obj, int instant)
        {
            byte[] buffer = new byte[obj.SerializationSize(instant)];
            int bufferOffset = 0;
            obj.Serialize(instant, buffer, ref bufferOffset);
            return buffer;
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
    }
}
