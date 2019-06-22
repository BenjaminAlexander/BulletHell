using MyGame.Engine.GameState.Instants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects
{
    public abstract class GenericGameObjectField<T> : GameObjectField
    {
        private T simulationValue;
        
        public T this[NextInstant next]
        {
            get
            {
                return simulationValue;
            }

            set
            {
                simulationValue = value;
            }
        }

        //TODO: if we don't actuall put the inital values we want in here, should the v argument be removed?
        public GenericGameObjectField(GameObject obj, T v) : base(obj)
        {
            simulationValue = v;
        }

        public static implicit operator T(GenericGameObjectField<T> m)
        {
            return m.simulationValue;
        }
        
        public override void ApplyMessage(GameObjectUpdate message)
        {
        }
    }
}
