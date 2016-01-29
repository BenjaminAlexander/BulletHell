using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public class IntegerGameObjectMember : NonInterpolatedGameObjectMember<int>
    {
        public IntegerGameObjectMember(GameObject obj, int v) : base(obj, v)
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            base.ApplyMessage(message);
            this.SimulationValue = message.ReadInt();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.SimulationValue);
            return message;
        }
    }
}
