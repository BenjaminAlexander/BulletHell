using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public class GameObjectReferenceField<T> : GenericGameObjectField<GameObjectReference<T>> where T : GameObject
    {
        private GameObjectCollection collection;

        public GameObjectReferenceField(GameObject obj, GameObjectReference<T> v, GameObjectCollection collection)
            : base(obj, v)
        {
            this.collection = collection;
        }

        public GameObjectReferenceField(GameObject obj, GameObjectCollection collection)
            : this(obj, new GameObjectReference<T>(null, collection), collection)
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.simulationValue = message.ReadGameObjectReference<T>(this.collection);
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.simulationValue);
            return message;
        }

        public override void Interpolate(float smoothing)
        {
            this.drawValue = this.simulationValue;
        }

        public T Dereference()
        {
            return this.Value.Dereference();
        }

        public Boolean CanDereference()
        {
            return this.Value.CanDereference();
        }
    }
}
