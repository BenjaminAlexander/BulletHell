using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MyGame.Engine.Reflection
{
    class TypeRefernce<BaseType>
    {
        private class ItemFactory<SubType> where SubType : BaseType, new()
        {
            public BaseType GetNewItem()
            {
                return new SubType();
            }
        }

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

        private Type[] subTypeArray;
        private Dictionary<Type, ConstructorInfo> constructorDictionary = new Dictionary<Type, ConstructorInfo>();
        
        public TypeRefernce(Type[] constructorParamsTypes)
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
            subTypeArray = types.ToArray();

            //check to make sure every object type has the required constructor
            for (int i = 0; i < subTypeArray.Length; i++)
            {
                System.Reflection.ConstructorInfo constructor = subTypeArray[i].GetConstructor(constructorParamsTypes);
                if (constructor == null)
                {
                    throw new MissingConstructorException(subTypeArray[i], constructorParamsTypes);
                }
                constructorDictionary[subTypeArray[i]] = constructor;
            }
        }

        public TypeRefernce() : this(new Type[0])
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

            for (int i = 0; i < subTypeArray.Length; i++)
            {
                if (subTypeArray[i] == t)
                {
                    return i;
                }
            }
            throw new Exception(t.Name +" is an unknown type of " + typeof(BaseType).Name);
        }

        public Type GetType(int id)
        {
            return subTypeArray[id];
        }

        public BaseType Construct(Type type, object[] constructorParams)
        {
            return (BaseType)constructorDictionary[type].Invoke(constructorParams);
        }

        public BaseType Construct(int type, object[] constructorParams)
        {
            return this.Construct(this.GetType(type), constructorParams);
        }
    }
}
