using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    abstract class NonInterpolatedGameObjectMember<T> : AbstractGameObjectMember<T> where T : struct
    {
        public override void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing)
        {
            NonInterpolatedGameObjectMember<T> myS = (NonInterpolatedGameObjectMember<T>)s;
            NonInterpolatedGameObjectMember<T> myD = (NonInterpolatedGameObjectMember<T>)d;

            this.Value = myS.Value;
        }

        public override void Interpolate(float smoothing)
        {
            this.drawValue = this.simulationValue;
        }

        public override void SetPrevious()
        {
            this.previousValue = this.drawValue;
        }
    }
}
