using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public class InterpolatedVector2GameObjectMember : GenericGameObjectField<Vector2>
    {
        public InterpolatedVector2GameObjectMember(GameObject obj, Vector2 v) : base(obj, v)
        {
        }

        public override void Interpolate(float smoothing)
        {
            this.drawValue = Vector2.Lerp(this.SimulationValue, this.previousValue, smoothing);
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.SimulationValue = message.ReadVector2();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.SimulationValue);
            return message;
        }
    }
}