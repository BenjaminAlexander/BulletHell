using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects
{
    public struct GameObjectReference<T> : FieldValue where T : GameObject
    {
        private int id;

        public int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        private GameObjectReference(T obj)
        {
            if (obj == null)
            {
                this.id = 0;
            }
            else
            {
                this.id = (int)obj.ID;
            }
        }

        public T Dereference(CurrentInstant current)
        {
            if(this.id == 0)
            {
                return null;
            }
            else
            {
                //TODO: fix this cast
                Engine.GameState.GameObject obj = current.GetObject(this.id);
                if(obj is T)
                {
                    return (T)obj;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Engine.Serialization.Utils.Write(this.id, buffer, ref bufferOffset);
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.id = Engine.Serialization.Utils.ReadInt(buffer, ref bufferOffset);
        }

        public static implicit operator GameObjectReference<T>(T obj)
        {
            return new GameObjectReference<T>(obj);
        }
    }
}
