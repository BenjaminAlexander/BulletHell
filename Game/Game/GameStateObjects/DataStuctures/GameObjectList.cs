using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects.DataStuctures
{
    class GameObjectList<T> : GameObjectListInterface where T : GameObject
    {
        List<T> list = new List<T>();

        public List<T> List
        {
            get { return list; }
        }

        public void Add(GameObject obj)
        {
            if (obj is T)
            {
                list.Add((T)obj);
            }
        }

        public Boolean Remove(GameObject obj)
        {
            if (obj is T)
            {
                return list.Remove((T)obj);
            }
            return false;
        }

        public Type GetListType()
        {
            return typeof(T);
        }

        public List<GameObject> GetList()
        {
            return list.Cast<GameObject>().ToList();
        }
    }
}
