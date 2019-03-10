using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private Dictionary<Type, int> typeToId = new Dictionary<Type, int>();
        private Dictionary<int, Type> idToType = new Dictionary<int, Type>();

        public void AddItem<DerivedType>() where DerivedType : BaseType, new()
        {
            if (!typeToId.ContainsKey(typeof(DerivedType)))
            {
                ObjectFactory<DerivedType> objectFactory = new ObjectFactory<DerivedType>();
                idToFactory[nextID] = objectFactory;
                idToType[nextID] = typeof(DerivedType);
                typeToId[typeof(DerivedType)] = nextID;
                nextID++;
            }
        }

        public int GetTypeID(BaseType obj)
        {
            return this.GetTypeID(obj.GetType());
        }

        public int GetTypeID(Type t)
        {
            return typeToId[t];
        }

        public Type GetTypeFromID(int id)
        {
            return idToType[id];
        }

        public BaseType Construct(Type type)
        {
            return this.Construct(typeToId[type]);
        }

        public BaseType Construct(int id)
        {
            return idToFactory[id].NewObject();
        }
    }
}
