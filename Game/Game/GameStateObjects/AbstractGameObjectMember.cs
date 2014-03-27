using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    abstract class AbstractGameObjectMember<T> : IGameObjectMember
    {
        T memberValue;

        public T Value
        {
            get { return memberValue; }
            set { memberValue = value; }
        }

        public abstract void ApplyMessage(GameObjectUpdate message);

        public abstract GameObjectUpdate ConstructMessage(GameObjectUpdate message);

        public abstract void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing);
    }
}
