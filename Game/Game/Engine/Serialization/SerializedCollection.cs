using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class SerializedCollection<BaseType> : LinkedSerializer<BaseType>, ICollection<BaseType>
    {
        int nextID = 0;
        TwoWayMap<int, BaseType> map = new TwoWayMap<int, BaseType>();
        TypeSerializer<BaseType> typeSerializer;

        public int Count
        {
            get
            {
                return map.Count;
            }
        }

        bool ICollection<BaseType>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public SerializedCollection(TypeSerializer<BaseType> typeSerializer) : base(typeSerializer)
        {
            this.typeSerializer = typeSerializer;
        }

        public SerializedCollection(TypeFactory<BaseType> factory, Serializer<BaseType> nestedSerializer) : this(new TypeSerializer<BaseType>(factory, nestedSerializer))
        {
            
        }

        internal int AddItem(BaseType obj)
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

        public bool Remove(BaseType obj)
        {
            return map.RemoveValue(obj);
        }

        internal BaseType GetObject(int id)
        {
            return map[id];
        }

        internal int GetID(BaseType obj)
        {
            return map[obj];
        }

        internal byte[] SerializeObject(int id)
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

        public void Add(BaseType item)
        {
            AddItem(item);
        }

        public void Clear()
        {
            nextID = 0;
            map.Clear();
        }

        public bool Contains(BaseType item)
        {
            return map.ContainsValue(item);
        }

        void ICollection<BaseType>.CopyTo(BaseType[] array, int arrayIndex)
        {
            if(array == null)
            {
                throw new ArgumentNullException();
            }

            if(arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            
            if(array.Length - arrayIndex < Count)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < Count; i++) ;

        }

        IEnumerator<BaseType> IEnumerable<BaseType>.GetEnumerator()
        {
            return map.Values.GetEnumerator();
        }

        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return map.Values.GetEnumerator();
        }
    }
}
