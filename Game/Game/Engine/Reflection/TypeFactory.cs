using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Reflection
{
    interface TypeFactory<BaseType>
    {
        int GetTypeID(BaseType obj);
        int GetTypeID(Type t);
        Type GetTypeFromID(int id);
        BaseType Construct<SubType>() where SubType : BaseType;
        BaseType Construct(Type type);
        BaseType Construct(int id);
    }
}
