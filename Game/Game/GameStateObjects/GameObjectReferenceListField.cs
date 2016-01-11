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
    public class GameObjectReferenceListField<T> : AbstractGameObjectField<List<GameObjectReference<T>>> where T : GameObject
    {
        private GameObjectCollection collection;

        public GameObjectReferenceListField(GameObject obj, List<GameObjectReference<T>> v, GameObjectCollection collection) : base(obj, v)
        {
            this.collection = collection;
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.simulationValue = message.ReadGameObjectReferenceList<T>(this.collection);
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
    }
}
