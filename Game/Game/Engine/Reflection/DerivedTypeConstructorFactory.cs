using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MyGame.Engine.Reflection
{
    class DerivedTypeConstructorFactory<BaseType>
    {
        public class MissingConstructorException : Exception
        {
            private static string BuildMessage(Type subType, Type[] constructorParamsTypes)
            {
                string message = "Subclass " + subType.Name + " of " + typeof(BaseType).Name + " does not have an constructor with expected parameters (";
                bool first = true;
                foreach (Type param in constructorParamsTypes)
                {
                    if (!first)
                    {
                        message = message + ", ";
                    }
                    message = message + param.Name;
                    first = false;
                }
                message = message + ")";
                return message;
            }

            public MissingConstructorException(Type subType, Type[] constructorParamsTypes) : base(BuildMessage(subType, constructorParamsTypes))
            {

            }
        }

        private Dictionary<int, Type> idToType = new Dictionary<int, Type>();
        private Dictionary<Type, int> typeToId = new Dictionary<Type, int>();
        private Dictionary<Type, ConstructorInfo> constructorDictionary = new Dictionary<Type, ConstructorInfo>();
        
        public DerivedTypeConstructorFactory(Type[] constructorParamsTypes)
        {
            IEnumerable<Type> types = null;
            //TODO: would it be better if the caller specified what assemblies it wanted to reference?
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(types == null)
                {
                    types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseType)));
                }
                else
                {
                    types = types.Concat(assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseType))));
                }
            }
            types = types.OrderBy(t => t.Name);
            Type[] subTypeArray = types.ToArray();

            //check to make sure every object type has the required constructor
            for (int i = 0; i < subTypeArray.Length; i++)
            {
                ConstructorInfo constructor = subTypeArray[i].GetConstructor(constructorParamsTypes);
                if (constructor == null)
                {
                    throw new MissingConstructorException(subTypeArray[i], constructorParamsTypes);
                }
                constructorDictionary[subTypeArray[i]] = constructor;
                idToType[i] = subTypeArray[i];
                typeToId[subTypeArray[i]] = i;
            }
        }

        public DerivedTypeConstructorFactory() : this(new Type[0])
        {
        }

        public int GetTypeID(BaseType obj)
        {
            return this.GetTypeID(obj.GetType());
        }

        public int GetTypeID(Type t)
        {
            if (!t.IsSubclassOf(typeof(BaseType)))
            {
                throw new Exception(t.Name + "is not a type of " + typeof(BaseType).Name);
            }

            try
            {
                return typeToId[t];
            }
            catch
            {
                throw new Exception(t.Name + " is an unknown type of " + typeof(BaseType).Name);
            }            
        }

        public Type GetTypeFromID(int id)
        {
            return idToType[id];
        }

        public BaseType Construct(Type type, object[] constructorParams)
        {
            return (BaseType)constructorDictionary[type].Invoke(constructorParams);
        }

        public BaseType Construct(int type, object[] constructorParams)
        {
            return this.Construct(this.GetTypeFromID(type), constructorParams);
        }
    }
}
