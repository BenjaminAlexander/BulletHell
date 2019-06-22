using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects
{
    public class InterpolatedVector2GameObjectMember : GenericGameObjectField<Vector2>
    {
        public InterpolatedVector2GameObjectMember(GameObject obj, Vector2 v) : base(obj, v)
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            base.ApplyMessage(message);
            this[new NextInstant(new Instant(0))] = message.ReadVector2();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this[new NextInstant(new Instant(0))]);
            return message;
        }
    }
}