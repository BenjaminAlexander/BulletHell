using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    public class GameObjectReference
    {
        private int id;
        private GameObject obj = null;
        private Boolean hasDereferenced = false;

        public GameObjectReference(GameObject obj)
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
                if (StaticGameObjectCollection.Collection.Contains(id))
                {
                    obj = StaticGameObjectCollection.Collection.Get(id);
                    hasDereferenced = true;
                }
                else
                {
                    hasDereferenced = false;
                }
            }
        }

        public GameObject Dereference()
        {
            if(hasDereferenced)
            {
                return obj;
            }
            else
            {
                if (StaticGameObjectCollection.Collection.Contains(id))
                {
                    obj = StaticGameObjectCollection.Collection.Get(id);
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
