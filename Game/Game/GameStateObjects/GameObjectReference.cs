using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Engine.GameState.FieldValues;

namespace MyGame.GameStateObjects
{
    public struct GameObjectReference<T> : FieldValue where T : GameObject
    {
        private int id;
        private T obj;
        private Boolean hasDereferenced;

        public int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        private GameObjectReference(T obj)
        {
            this.obj = obj;
            this.hasDereferenced = true;
            if (obj == null)
            {
                this.id = 0;
            }
            else
            {
                this.id = obj.ID;
            }
        }

        public GameObjectReference(GameObjectUpdate message, GameObjectCollection collection)
        {
            this.obj = null;
            this.hasDereferenced = false;
            this.id = message.ReadInt();
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

            if (GameObjectCollection.Reference.Contains(id))
            {
                obj = (T)GameObjectCollection.Reference.Get(id);
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

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Engine.Serialization.Utils.Write(this.id, buffer, ref bufferOffset);
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.id = Engine.Serialization.Utils.ReadInt(buffer, ref bufferOffset);
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
