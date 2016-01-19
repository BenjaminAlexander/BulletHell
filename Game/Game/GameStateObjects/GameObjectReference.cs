using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public struct GameObjectReference<T> where T : GameObject
    {
        private int id;
        private GameObjectCollection collection;
        private T obj;
        private Boolean hasDereferenced;

        public GameObjectReference(T obj, GameObjectCollection collection)
        {
            this.obj = obj;
            this.collection = collection;
            this.hasDereferenced = true;
            if (obj == null)
            {
                id = 0;
            }
            else
            {
                id = obj.ID;
            }
        }

        public GameObjectReference(int id, GameObjectCollection collection)
        {
            this.obj = null;
            this.hasDereferenced = false;
            this.id = id;
            this.collection = collection;
            if (id == 0)
            {
                this.obj = null;
                hasDereferenced = true;
            }
            else
            {
                Dereference();
            }
        }

        private T Dereference()
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

        public static implicit operator T(GameObjectReference<T> reference)
        {
            return reference.Dereference();
        }

        public static implicit operator GameObjectReference<T>(T obj)
        {
            return new GameObjectReference<T>(obj, obj.Game.GameObjectCollection);
        }
    }
}
