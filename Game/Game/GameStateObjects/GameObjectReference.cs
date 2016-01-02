using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public class GameObjectReference<T> where T : GameObject
    {
        private int id;
        private GameObjectCollection collection;
        private T obj = null;
        private Boolean hasDereferenced = false;

        public GameObjectReference(T obj, GameObjectCollection collection)
        {
            this.obj = obj;
            this.collection = collection;
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

        public GameObjectReference(int id, GameObjectCollection collection)
        {
            this.id = id;
            this.collection = collection;
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
                if (this.collection.Contains(id))
                {
                    GameObject pObj = this.collection.Get(id);
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
