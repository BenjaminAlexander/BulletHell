using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    // This class is used to identify game objects with unique integers, so that references to particular objects can be encoded in update messages.
    // Additionally, it allows clients to reference objects that do not exist yet in messages to the server.
    public class GameObjectReference<T> where T : GameObject
    {
        // The unique id of a game object.
        private int id;

        // The game object.  Defaults to null, and is only retreived when needed.
        private T obj = null;

        // 
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
            
            return null;
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
