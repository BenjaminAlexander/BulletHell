using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.GameState
{
    partial class GameObject : SerializableInstant
    {
        private const int OBJECT_ID_LOCATION = 0;
        private const int TYPE_ID_LOCATION = sizeof(int);
        private const int INSTANT_LOCATION = sizeof(int) * 2;
        private const int HEADER_SIZE = sizeof(int) * 3;
        private static DerivedTypeConstructorFactory<GameObject> typeReference = new DerivedTypeConstructorFactory<GameObject>();
        private static int nextID = 0;

        public static int GetObjectIDFromSerialization(byte[] buffer, int bufferOffset)
        {
            return BitConverter.ToInt32(buffer, bufferOffset + OBJECT_ID_LOCATION);
        }

        private static int GetTypeIDFromSerialization(byte[] buffer, int bufferOffset)
        {
            return BitConverter.ToInt32(buffer, bufferOffset + TYPE_ID_LOCATION);
        }

        public static Instant GetInstantFromSerialization(byte[] buffer, int bufferOffset)
        {
            return Serialization.Utils.Deserialize<Instant>(buffer, ref bufferOffset);
        }

        //private int id;
        private int typeID;
        private int serializationSize = sizeof(int) * 3;
        private List<Field> fields = new List<Field>();
        
        public GameObject()
        {
            //this.id = nextID;
            nextID++;
            typeID = typeReference.GetTypeID(this);
        }

        public int SerializationSize
        {
            get
            {
                return serializationSize;
            }
        }

        private int AddField(Field field)
        {
            int bufferAddress = serializationSize;
            serializationSize = serializationSize + field.Size;
            this.fields.Add(field);
            return bufferAddress;
        }

        public void Serialize(Instant instant, byte[] buffer, int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.serializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }
            //Buffer.BlockCopy(BitConverter.GetBytes(this.id), 0, buffer, bufferOffset + OBJECT_ID_LOCATION, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(this.typeID), 0, buffer, bufferOffset + TYPE_ID_LOCATION, sizeof(int));
            instant.Serialize(buffer, bufferOffset + INSTANT_LOCATION);
            bufferOffset = HEADER_SIZE;
            foreach (Field field in fields)
            {
                field.Serialize(instant, buffer, bufferOffset);
                bufferOffset = bufferOffset + field.Size;
            }
        }

        private void ForceDeserialize(int id, Instant instant, byte[] buffer, int bufferOffset)
        {
            //this.id = id;
            if (buffer.Length - bufferOffset < this.serializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            int messageTypeID = GameObject.GetTypeIDFromSerialization(buffer, bufferOffset);
            if (messageTypeID != this.typeID)
            {
                throw new Exception("The Message ID does not match this object ID.");
            }

            bufferOffset = bufferOffset + HEADER_SIZE;
            foreach (Field field in fields)
            {
                field.Deserialize(instant, buffer, bufferOffset);
                bufferOffset = bufferOffset + field.Size;
            }
        }

        public void Deserialize(Instant instant, byte[] buffer, int bufferOffset)
        {
            int messageObjectID = GameObject.GetObjectIDFromSerialization(buffer, bufferOffset);
            /*if(messageObjectID != this.id)
            {
                throw new Exception("The Message ID does not match this object ID.");
            }*/

            ForceDeserialize(messageObjectID, instant, buffer, bufferOffset);
        }
    }
}
