using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class InterpolatedAngleGameObjectMember : AbstractGameObjectField<float>
    {
        public InterpolatedAngleGameObjectMember(GameObject obj, float v)
            : base(obj, v)
        {
        }

        public override void Interpolate(float smoothing)
        {
            this.drawValue = Utils.Vector2Utils.Lerp(this.simulationValue, this.previousValue, smoothing);
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.simulationValue = message.ReadFloat();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.simulationValue);
            return message;
        }

        public override void SetPrevious()
        {
            this.previousValue = this.drawValue;
        }
    }
}
