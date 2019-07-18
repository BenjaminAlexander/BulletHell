using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects.DataStuctures
{
    public class ListManager<BaseType>
    {
        BaseTypeListInterface<BaseType> master = new SubTypeList<BaseType, BaseType>();
        List<BaseTypeListInterface<BaseType>> listOfLists = new List<BaseTypeListInterface<BaseType>>();

        public ListManager()
        {
        }

        public List<Subtype> GetList<Subtype>() where Subtype : BaseType
        {
            if (master.GetListType() == typeof(Subtype))
            {
                return master.GetList().Cast<Subtype>().ToList();
            }

            foreach (BaseTypeListInterface<BaseType> list in listOfLists)
            {
                if (list.GetListType() == typeof(Subtype))
                {
                    return list.GetList().Cast<Subtype>().ToList();
                }
            }
            BaseTypeListInterface<BaseType> newList = new SubTypeList<BaseType, Subtype>();
            foreach(BaseType obj in master.GetList())
            {
                newList.Add(obj);
            }
            listOfLists.Add(newList);
            return newList.GetList().Cast<Subtype>().ToList();
        }

        public void Add(BaseType obj)
        {
            master.Add(obj);
            foreach (BaseTypeListInterface<BaseType> list in listOfLists)
            {
                list.Add(obj);
            }
        }

        public void Remove(BaseType obj)
        {
            master.Remove(obj);
            foreach (BaseTypeListInterface<BaseType> list in listOfLists)
            {
                list.Remove(obj);
            }
        }

        public List<BaseType> GetMaster()
        {
            return master.GetList();
        }

        public Boolean Contains(BaseType obj)
        {
            return master.GetList().Contains(obj);
        }
    }
}
