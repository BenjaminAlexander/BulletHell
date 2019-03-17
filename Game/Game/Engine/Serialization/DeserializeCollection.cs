using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class DeserializeCollection<BaseType>
    {
        TwoWayMap<int, BaseType> map;
        TypeDeserializer<BaseType> typeDeserializer;

        public DeserializeCollection(TwoWayMap<int, BaseType> map, TypeFactory<BaseType> factory)
        {
            this.map = map;
            this.typeDeserializer = new TypeDeserializer<BaseType>(factory);
        }

        public DeserializeCollection(TypeFactory<BaseType> factory) : this(new TwoWayMap<int, BaseType>(), factory)
        {

        }

        public BaseType GetObject(int id)
        {
            return map[id];
        }

        public int GetID(BaseType obj)
        {
            return map[obj];
        }

        public int DeserializeObject(Deserializer<BaseType> deserializer, byte[] buffer)
        {
            int bufferOffset = 0;
            return this.DeserializeObject(deserializer, buffer, ref bufferOffset);
        }

        public int DeserializeObject(Deserializer<BaseType> deserializer, byte[] buffer, ref int bufferOffset)
        {
            int objectId = Utils.ReadInt(buffer, ref bufferOffset);
            if (map.ContainsKey(objectId))
            {
                typeDeserializer.Deserialize(deserializer, map[objectId], buffer, ref bufferOffset);
            }
            else
            {
                BaseType newObject = typeDeserializer.Deserialize(deserializer, buffer, ref bufferOffset);
                map.Set(objectId, newObject);
            }
            return objectId;
        }
    }
}
