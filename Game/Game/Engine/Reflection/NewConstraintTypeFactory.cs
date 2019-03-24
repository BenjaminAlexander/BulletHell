using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;

namespace MyGame.Engine.Reflection
{
    class NewConstraintTypeFactory<BaseType> : TypeFactory<BaseType>
    {
        interface ObjectFactoryInterface
        {
            BaseType NewObject();
        }

        class ObjectFactory<DerivedType> : ObjectFactoryInterface where DerivedType : BaseType, new()
        {
            public BaseType NewObject()
            {
                return new DerivedType();
            }
        }

        private int nextID = 0;
        private Dictionary<int, ObjectFactoryInterface> idToFactory = new Dictionary<int, ObjectFactoryInterface>();
        private TwoWayMap<int, Type> map = new TwoWayMap<int, Type>();

        public void AddType<DerivedType>() where DerivedType : BaseType, new()
        {
            if (!map.ContainsValue(typeof(DerivedType)))
            {
                ObjectFactory<DerivedType> objectFactory = new ObjectFactory<DerivedType>();
                idToFactory[nextID] = objectFactory;
                map[nextID] = typeof(DerivedType);
                nextID++;
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

        public BaseType Construct(Type type)
        {
            return this.Construct(map[type]);
        }

        public BaseType Construct(int id)
        {
            BaseType newObject = idToFactory[id].NewObject();
            this.OnConstruct(newObject);
            return newObject;
        }

        public virtual void OnConstruct(BaseType newObject)
        {

        }
    }
}
