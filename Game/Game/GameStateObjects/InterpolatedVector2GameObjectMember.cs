﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class InterpolatedVector2GameObjectMember : AbstractGameObjectMember<Vector2>
    {
        public InterpolatedVector2GameObjectMember(Vector2 v)
        {
            this.Value = v;
        }

        public override void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing)
        {
            InterpolatedVector2GameObjectMember myS = (InterpolatedVector2GameObjectMember)s;
            InterpolatedVector2GameObjectMember myD = (InterpolatedVector2GameObjectMember)d;

            this.Value = Vector2.Lerp(myS.Value, myD.Value, smoothing);
        }

        public override void Interpolate(float smoothing)
        {
            this.drawValue = Vector2.Lerp(this.simulationValue, this.previousValue, smoothing);
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.simulationValue = message.ReadVector2();
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