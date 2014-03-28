using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public interface IGameObjectMember
    {
        void ApplyMessage(GameObjectUpdate message);

        GameObjectUpdate ConstructMessage(GameObjectUpdate message);

        void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing);

        void Interpolate(float smoothing);

        void SetPrevious();

        GameObject Obj
        {
            get;
            set;
        }
    }
}
