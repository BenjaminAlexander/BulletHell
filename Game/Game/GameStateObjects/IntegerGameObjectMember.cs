using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class IntegerGameObjectMember : NonInterpolatedGameObjectMember<int>
    {
        public IntegerGameObjectMember(int v)
        {
            this.Value = v;
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.simulationValue = message.ReadInt();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.simulationValue);
            return message;
        }
    }
}
