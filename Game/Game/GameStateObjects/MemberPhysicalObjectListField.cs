using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class GameObjectReferenceListField<T> : AbstractGameObjectMember<List<GameObjectReference<T>>> where T : GameObject
    {
        public GameObjectReferenceListField(List<GameObjectReference<T>> v)
        {
            this.Value = v;
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            Game1.AssertIsNotServer();
            this.simulationValue = message.ReadGameObjectReferenceList<T>();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            Game1.AsserIsServer();
            message.Append(this.simulationValue);
            return message;
        }

        public override void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing)
        {
            GameObjectReferenceListField<T> myS = (GameObjectReferenceListField<T>)s;
            GameObjectReferenceListField<T> myD = (GameObjectReferenceListField<T>)d;

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
