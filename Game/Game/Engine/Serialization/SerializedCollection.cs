﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class SerializedCollection<BaseType>
    {
        int nextID = 0;
        TwoWayMap<int, BaseType> map;
        TypeSerializer<BaseType> wipTypeSerializer;

        public SerializedCollection(TwoWayMap<int, BaseType> map, 
            TypeFactory<BaseType> factory, 
            Serializer<BaseType> nestedSerializer, 
            Deserializer<BaseType> nestedDeserializer)
        {
            this.map = map;
            wipTypeSerializer = new TypeSerializer<BaseType>(factory, nestedSerializer, nestedDeserializer);
        }

        public SerializedCollection(TypeFactory<BaseType> factory, 
            Serializer<BaseType> nestedSerializer, 
            Deserializer<BaseType> nestedDeserializer) 
            : this(new TwoWayMap<int, BaseType>(), 
                factory, 
                nestedSerializer, 
                nestedDeserializer)
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

        public BaseType GetObject(int id)
        {
            return map[id];
        }

        public int GetID(BaseType obj)
        {
            return map[obj];
        }

        public int ObjectSerializationSize(int id)
        {
            return this.ObjectSerializationSize(map[id]);
        }

        public int ObjectSerializationSize(BaseType obj)
        {
            return wipTypeSerializer.SerializationSize(obj) + sizeof(int);
        }

        public void SerializeObject(int id, byte[] buffer, ref int bufferOffset)
        {
            SerializeObject(id, map[id], buffer, ref bufferOffset);
        }

        public void SerializeObject(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            SerializeObject(map[obj], obj, buffer, ref bufferOffset);
        }

        private void SerializeObject(int id, BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(id), 0, buffer, bufferOffset, sizeof(int));
            bufferOffset = bufferOffset + sizeof(int);
            wipTypeSerializer.Serialize(obj, buffer, ref bufferOffset);
        }

        public byte[] SerializeObject(int id)
        {
            byte[] serialization = new byte[ObjectSerializationSize(id)];
            int offset = 0;
            this.SerializeObject(id, serialization, ref offset);
            return serialization;
        }

        public byte[] SerializeObject(BaseType obj)
        {
            byte[] serialization = new byte[ObjectSerializationSize(obj)];
            int offset = 0;
            this.SerializeObject(obj, serialization, ref offset);
            return serialization;
        }

        public int DeserializeObject(byte[] buffer)
        {
            int bufferOffset = 0;
            return this.DeserializeObject(buffer, ref bufferOffset);
        }

        public int DeserializeObject(byte[] buffer, ref int bufferOffset)
        {
            int objectId = Utils.ReadInt(buffer, ref bufferOffset);
            if (map.ContainsKey(objectId))
            {
                wipTypeSerializer.Deserialize(map[objectId], buffer, ref bufferOffset);
            }
            else
            {
                BaseType newObject = wipTypeSerializer.Deserialize(buffer, ref bufferOffset);
                map.Set(objectId, newObject);
            }
            return objectId;
        }
    }
}
