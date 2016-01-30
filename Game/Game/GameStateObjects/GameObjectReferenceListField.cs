using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public class GameObjectReferenceListField<T> : GameObjectField where T : GameObject
    {
        private GameObjectCollection collection;
        private List<GameObjectReference<T>> value;

        public GameObjectReferenceListField(GameObject obj) : base(obj)
        {
            this.collection = obj.Game.GameObjectCollection;
            this.value = new List<GameObjectReference<T>>();
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            var rtn = new List<GameObjectReference<T>>();
            int count = message.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GameObjectReference<T> rf = new GameObjectReference<T>(message, collection);
                rtn.Add(rf);
            }
            this.value = rtn;
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.value.Count);
            foreach (GameObjectReference<T> obj in this.value)
            {
                message = obj.ConstructMessage(message);
            }
            return message;
        }

        public List<GameObjectReference<T>> Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
