using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects.DataStuctures
{
    class SubTypeList<BaseType, SubType> : BaseTypeListInterface<BaseType> where SubType : BaseType
    {
        List<SubType> list = new List<SubType>();

        public List<SubType> List
        {
            get { return list; }
        }

        public void Add(BaseType obj)
        {
            if (obj is SubType)
            {
                list.Add((SubType)obj);
            }
        }

        public Boolean Remove(BaseType obj)
        {
            if (obj is SubType)
            {
                return list.Remove((SubType)obj);
            }
            return false;
        }

        public Type GetListType()
        {
            return typeof(SubType);
        }

        public List<BaseType> GetList()
        {
            return list.Cast<BaseType>().ToList();
        }
    }
}
