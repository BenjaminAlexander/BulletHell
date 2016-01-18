using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class GenericGameObjectField<T> : GameObjectField
    {
        internal T simulationValue;
        internal T previousValue;
        internal T drawValue;

        public GenericGameObjectField(GameObject obj, T v)
        {
            simulationValue = v;
            drawValue = v;
            previousValue = v;
            obj.AddField(this);
        }

        public static implicit operator T(GenericGameObjectField<T> m)
        {
            return m.Value;
        }
        
        public T Value
        {
            get 
            {
                if (GameObjectField.IsModeSimulation())
                {
                    return simulationValue;
                }
                else if (GameObjectField.IsModePrevious())
                {
                    return previousValue;
                }
                else
                {
                    return drawValue;
                }
            }
            set 
            {
                if (GameObjectField.IsModeSimulation())
                {
                    simulationValue = value;
                }
                else if (GameObjectField.IsModePrevious())
                {
                    previousValue = value;
                }
                else
                {
                    drawValue = value;
                }
            }
        }

        public override void SetPrevious()
        {
            this.previousValue = this.drawValue;
        }

        public override void SetAllToSimulation()
        {
            this.previousValue = this.simulationValue;
            this.drawValue = this.simulationValue;
        }
    }
}
