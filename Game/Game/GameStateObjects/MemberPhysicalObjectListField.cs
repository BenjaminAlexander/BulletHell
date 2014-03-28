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
            this.Value = message.ReadGameObjectReferenceList<T>();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            Game1.AsserIsServer();
            message.Append(this.Value);
            return message;
        }

        public override void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing)
        {
            GameObjectReferenceListField<T> myS = (GameObjectReferenceListField<T>)s;
            GameObjectReferenceListField<T> myD = (GameObjectReferenceListField<T>)d;

            this.Value = myS.Value;
        }
    }
}
