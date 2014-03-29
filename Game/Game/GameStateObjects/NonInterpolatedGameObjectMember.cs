using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    abstract class NonInterpolatedGameObjectMember<T> : AbstractGameObjectField<T> where T : struct
    {
        public NonInterpolatedGameObjectMember(GameObject obj, T v)
            : base(obj, v)
        {
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
