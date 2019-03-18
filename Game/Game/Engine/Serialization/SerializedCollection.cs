using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class SerializedCollection<BaseType> : LinkedSerializer<BaseType>
    {
        int nextID = 0;
        TwoWayMap<int, BaseType> map = new TwoWayMap<int, BaseType>();
        TypeSerializer<BaseType> typeSerializer;

        public SerializedCollection(TypeSerializer<BaseType> typeSerializer) : base(typeSerializer)
        {
            this.typeSerializer = typeSerializer;
        }

        public SerializedCollection(TypeFactory<BaseType> factory, Serializer<BaseType> nestedSerializer) : this(new TypeSerializer<BaseType>(factory, nestedSerializer))
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

        public byte[] SerializeObject(int id)
        {
            return Utils.Serialize<BaseType>(this, map[id]);
        }

        public BaseType Deserialize(byte[] buffer)
        {
            int bufferOffset = 0;
            return this.Deserialize(buffer, ref bufferOffset);
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            BaseType obj = GetDeserializedObject(buffer, bufferOffset);
            this.Deserialize(obj, buffer, ref bufferOffset);
            return obj;
        }

        public BaseType GetDeserializedObject(byte[] buffer, int bufferOffset)
        {
            int objectId = Utils.ReadInt(buffer, ref bufferOffset);
            if (map.ContainsKey(objectId))
            {
                return map[objectId];
            }
            else
            {
                BaseType newObject = typeSerializer.Construct(buffer, bufferOffset);
                map.Set(objectId, newObject);
                return newObject;
            }
        }

        protected override int AdditionalSerializationSize(BaseType obj)
        {
            return sizeof(int);
        }

        protected override void AdditionalDeserialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            int objectId = Utils.ReadInt(buffer, ref bufferOffset);
            if(objectId != map[obj])
            {
                throw new Exception("Serialized object ID does not match object ID");
            }
        }

        protected override void AdditionalSerialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(map[obj]), 0, buffer, bufferOffset, sizeof(int));
            bufferOffset = bufferOffset + sizeof(int);
        }
    }
}
