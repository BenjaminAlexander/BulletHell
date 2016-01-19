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

        public GameObjectReferenceField(GameObject obj, GameObjectCollection collection)
            : base(obj, new GameObjectReference<T>(null, collection))
        {
            this.collection = collection;
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            GameObjectReference<T> rf = new GameObjectReference<T>(message.ReadInt(), this.collection);
            this.simulationValue = rf;
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.simulationValue.ID);
            return message;
        }

        public override void Interpolate(float smoothing)
        {
            this.drawValue = this.simulationValue;
        }

        public T Dereference()
        {
            return this.Value;
        }

        public Boolean CanDereference()
        {
            return this.Value.CanDereference();
        }
    }
}
