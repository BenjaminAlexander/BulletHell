using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    public class GameObjectReference<T> where T : GameObject
    {
        private int id;
        private T obj = null;
        private Boolean hasDereferenced = false;

        public GameObjectReference(T obj)
        {
            this.obj = obj;
            if (obj == null)
            {
                id = 0;
            }
            else
            {
                id = obj.ID;
            }
            hasDereferenced = true;
        }

        public GameObjectReference(int id)
        {
            this.id = id;
            if (id == 0)
            {
                obj = null;
                hasDereferenced = true;
            }
            else
            {
                Dereference();
            }
        }

        public T Dereference()
        {
            if(hasDereferenced)
            {
                return obj;
            }
            else
            {
                if (StaticGameObjectCollection.Collection.Contains(id))
                {
                    GameObject pObj = StaticGameObjectCollection.Collection.Get(id);
                    if (pObj is T)
                    {
                        obj = (T)pObj;
                    }
                    hasDereferenced = true;
                    return obj;
                }
                else
                {
                    return null;
                }
            }
        }

        public Boolean CanDereference()
        {
            Dereference();
            return hasDereferenced;
        }

        public int ID
        {
            get { return id; }
        }
    }
}
