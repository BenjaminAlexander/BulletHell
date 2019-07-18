using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects.DataStuctures
{
    interface BaseTypeListInterface<BaseType>
    {
        void Add(BaseType obj);
        Boolean Remove(BaseType obj);
        Type GetListType();
        List<BaseType> GetList();
    }
}
