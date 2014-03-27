using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class InterpolatedAngleGameObjectMember : AbstractGameObjectMember<float>
    {
        public InterpolatedAngleGameObjectMember(float v)
        {
            this.Value = v;
        }

        public override void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing)
        {
            InterpolatedAngleGameObjectMember myS = (InterpolatedAngleGameObjectMember)s;
            InterpolatedAngleGameObjectMember myD = (InterpolatedAngleGameObjectMember)d;

            this.Value = Utils.Vector2Utils.Lerp(myS.Value, myD.Value, smoothing);
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.Value = message.ReadFloat();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.Value);
            return message;
        }
    }
}
