using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public interface IGameObjectField
    {
        void ApplyMessage(GameObjectUpdate message);

        GameObjectUpdate ConstructMessage(GameObjectUpdate message);

        void Interpolate(float smoothing);

        void SetPrevious();
    }
}
