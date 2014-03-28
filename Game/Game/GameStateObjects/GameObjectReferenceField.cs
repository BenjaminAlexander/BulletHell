using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class GameObjectReferenceField<T> : AbstractGameObjectMember<GameObjectReference<T>> where T : GameObject
    {
        public GameObjectReferenceField(GameObjectReference<T> v)
        {
            this.Value = v;
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

        public override void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing)
        {
            GameObjectReferenceField<T> myS = (GameObjectReferenceField<T>)s;

            this.Value = myS.Value;
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
