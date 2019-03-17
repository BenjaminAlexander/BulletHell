using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class SerializeCollection<BaseType>
    {
        int nextID = 0;
        TwoWayMap<int, BaseType> map;
        TypeSerializer<BaseType> typeSerializer;

        public SerializeCollection(TwoWayMap<int, BaseType> map, TypeFactory<BaseType> factory)
        {
            this.map = map;
            this.typeSerializer = new TypeSerializer<BaseType>(factory);
        }

        public SerializeCollection(TypeFactory<BaseType> factory) : this(new TwoWayMap<int, BaseType>(), factory)
        {

        }

        public int Add(BaseType obj)
        {
            if (!map.ContainsValue(obj))
            {
                int id = nextID;
                nextID++;
                map.Set(id, obj);
                return id;
            }
            else
            {
                return map[obj];
            }
        }

        public int ObjectSerializationSize(Serializer<BaseType> serializer, int id)
        {
            return this.ObjectSerializationSize(serializer, map[id]);
        }

        public int ObjectSerializationSize(Serializer<BaseType> serializer, BaseType obj)
        {
            return serializer.SerializationSize(obj) + sizeof(int);
        }

        public void SerializeObject(Serializer<BaseType> serializer, int id, byte[] buffer, int bufferOffset)
        {
            SerializeObject(serializer, id, map[id], buffer, bufferOffset);
        }

        public void SerializeObject(Serializer<BaseType> serializer, BaseType obj, byte[] buffer, int bufferOffset)
        {
            SerializeObject(serializer, map[obj], obj, buffer, bufferOffset);
        }

        private void SerializeObject(Serializer<BaseType> serializer, int id, BaseType obj, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(id), 0, buffer, bufferOffset, sizeof(int));
            serializer.Serialize(obj, buffer, bufferOffset + sizeof(int));
        }

        public byte[] SerializeObject(Serializer<BaseType> serializer, int id)
        {
            byte[] serialization = new byte[ObjectSerializationSize(serializer, id)];
            this.SerializeObject(serializer, id, serialization, 0);
            return serialization;
        }

        public byte[] SerializeObject(Serializer<BaseType> serializer, BaseType obj)
        {
            byte[] serialization = new byte[ObjectSerializationSize(serializer, obj)];
            this.SerializeObject(serializer, obj, serialization, 0);
            return serialization;
        }
    }
}
