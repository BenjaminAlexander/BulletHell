using MyGame.Engine.GameState.Instants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    public class FloatGameObjectMember : NonInterpolatedGameObjectMember<float>
    {
        public FloatGameObjectMember(GameObject obj, float v)
            : base(obj, v)
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            base.ApplyMessage(message);
            this[new NextInstant(new Instant(0))] = message.ReadFloat();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this[new NextInstant(new Instant(0))]);
            return message;
        }
    }
}