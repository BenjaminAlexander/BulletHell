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

        private GameObjectReference(T obj)
        {
            this.obj = obj;
            this.hasDereferenced = true;
            if (obj == null)
            {
                this.id = 0;
                this.collection = null;
            }
            else
            {
                this.id = obj.ID;
                this.collection = obj.Game.GameObjectCollection;
            }
        }

        public GameObjectReference(GameObjectUpdate message, GameObjectCollection collection)
        {
            this.obj = null;
            this.hasDereferenced = false;
            this.id = message.ReadInt();
            this.collection = collection;
            if (id == 0)
            {
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

            if (this.collection.Contains(id))
            {
                obj = (T)this.collection.Get(id);
                hasDereferenced = true;
                return obj;
            }
            else
            {
                return null;
            }
        }

        public GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.id);
            return message;
        }

        public Boolean IsDereferenced()
        {
            Dereference();
            return hasDereferenced;
        }

        public static implicit operator T(GameObjectReference<T> reference)
        {
            return reference.Dereference();
        }

        public static implicit operator GameObjectReference<T>(T obj)
        {
            return new GameObjectReference<T>(obj);
        }
    }
}
