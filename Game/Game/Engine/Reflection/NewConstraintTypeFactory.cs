using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;

namespace MyGame.Engine.Reflection
{
    public class NewConstraintTypeFactory<BaseType> : TypeFactory<BaseType>
    {
        interface MetaDataInterface
        {
            BaseType NewObject();
        }

        class MetaData<DerivedType> : MetaDataInterface where DerivedType : BaseType, new()
        {
            public BaseType NewObject()
            {
                return new DerivedType();
            }
        }

        private Dictionary<int, MetaDataInterface> metaData = new Dictionary<int, MetaDataInterface>();
        private TwoWayMap<int, Type> map = new TwoWayMap<int, Type>();

        public void AddType<DerivedType>() where DerivedType : BaseType, new()
        {
            if (!map.ContainsValue(typeof(DerivedType)))
            {
                int nextID = map.GreatestKey + 1;
                MetaData<DerivedType> objectFactory = new MetaData<DerivedType>();
                metaData[nextID] = objectFactory;
                map[nextID] = typeof(DerivedType);
            }
        }

        public int GetTypeID(BaseType obj)
        {
            return this.GetTypeID(obj.GetType());
        }

        public int GetTypeID(Type t)
        {
            return map[t];
        }

        public Type GetTypeFromID(int id)
        {
            return map[id];
        }

        public BaseType Construct<SubType>() where SubType : BaseType
        {
            return Construct(typeof(SubType));
        }

        public BaseType Construct(Type type)
        {
            return this.Construct(map[type]);
        }

        public BaseType Construct(int id)
        {
            BaseType newObject = metaData[id].NewObject();
            return newObject;
        }
    }
}
