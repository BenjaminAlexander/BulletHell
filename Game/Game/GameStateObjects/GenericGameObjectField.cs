using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class GenericGameObjectField<T> : GameObjectField
    {
        private T simulationValue;
        internal T previousValue;
        internal T drawValue;

        //TODO: is this method the best?
        private bool initialized = false;
        
        //TODO: make this clean
        protected T SimulationValue
        {
            get
            {
                return simulationValue;
            }

            set
            {
                simulationValue = value;
                if (!initialized)
                {
                    previousValue = value;
                    drawValue = value;
                    initialized = true;
                }
            }
        }
            
        //TODO: if we don't actuall put the inital values we want in here, should the v argument be removed?
        public GenericGameObjectField(GameObject obj, T v) : base(obj)
        {
            simulationValue = v;
            drawValue = v;
            previousValue = v;
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
    }
}
