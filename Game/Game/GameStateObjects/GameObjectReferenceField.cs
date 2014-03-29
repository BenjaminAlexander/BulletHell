using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class GameObjectReferenceField<T> : AbstractGameObjectField<GameObjectReference<T>> where T : GameObject
    {
        public GameObjectReferenceField(GameObject obj, GameObjectReference<T> v) : base(obj, v)
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.simulationValue = message.ReadGameObjectReference<T>();
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

        public override void SetPrevious()
        {
            this.previousValue = this.drawValue;
        }
    }
}
