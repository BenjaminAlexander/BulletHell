using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Engine.GameState.FieldValues;

namespace MyGame.GameStateObjects
{
    public struct GameObjectReferenceListField<T> : FieldValue where T : GameObject
    {
        private List<GameObjectReference<T>> value;

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            if (value == null)
            {
                value = new List<GameObjectReference<T>>();
            }
            Engine.Serialization.Utils.Write(value.Count, buffer, ref bufferOffset);
            foreach (GameObjectReference<T> reference in value)
            {
                reference.Serialize(buffer, ref bufferOffset);
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            value = new List<GameObjectReference<T>>();
            int count = Engine.Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            for(int i = 0; i < count; i++)
            {
                GameObjectReference<T> reference = new GameObjectReference<T>();
                reference.Deserialize(buffer, ref bufferOffset);
                value.Add(reference);
            }
        }

        public void Add(GameObjectReference<T> reference)
        {
            if(value == null)
            {
                value = new List<GameObjectReference<T>>();
            }
            value.Add(reference);
        }

        public GameObjectReference<T> Get(int index)
        {
            if (value == null)
            {
                value = new List<GameObjectReference<T>>();
            }
            return this.value[index];
        }

        public void Set(int index, GameObjectReference<T> reference)
        {
            if (value == null)
            {
                value = new List<GameObjectReference<T>>();
            }
            this.value[index] = reference;
        }

        public int SerializationSize
        {
            get
            {
                if (value == null)
                {
                    value = new List<GameObjectReference<T>>();
                }

                int size = sizeof(int);
                foreach (GameObjectReference<T> reference in value)
                {
                    size = size + reference.SerializationSize;
                }
                return size;
            }
        }
    }
}
