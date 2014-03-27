﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class FloatGameObjectMember : NonInterpolatedGameObjectMember<float>
    {
        public FloatGameObjectMember(float v)
        {
            this.Value = v;
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