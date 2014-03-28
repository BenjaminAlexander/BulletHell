using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    abstract class AbstractGameObjectMember<T> : IGameObjectMember
    {
        internal T simulationValue;
        internal T previousValue;
        internal T drawValue;

        private GameObject obj;
        public GameObject Obj
        {
            set { obj = value; }
            get
            {
                return obj;
            }
        }
        
        public T Value
        {
            get 
            {
                if (obj == null)
                {
                    return (new SimulationSelctor()).SelectValue<T>(this); 
                }
                return obj.Mode.SelectValue<T>(this); 
            }
            set 
            {
                if (obj == null)
                {
                    (new SimulationSelctor()).SetValue<T>(this, value);
                }
                else
                {
                    obj.Mode.SetValue<T>(this, value);
                }
            }
        }

        public abstract void ApplyMessage(GameObjectUpdate message);

        public abstract GameObjectUpdate ConstructMessage(GameObjectUpdate message);

        public abstract void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing);

        public abstract void Interpolate(float smoothing);


        public abstract void SetPrevious();
    }

    public abstract class ValueSelctor
    {
        internal abstract R SelectValue<R>(AbstractGameObjectMember<R> field);
        internal abstract void SetValue<R>(AbstractGameObjectMember<R> field, R value);
    }

    public class SimulationSelctor : ValueSelctor
    {
        internal override R SelectValue<R>(AbstractGameObjectMember<R> field)
        {
            return field.simulationValue;
        }

        internal override void SetValue<R>(AbstractGameObjectMember<R> field, R value)
        {
            field.simulationValue = value;
        }
    }

    public class PreviousSelctor : ValueSelctor
    {
        internal override R SelectValue<R>(AbstractGameObjectMember<R> field)
        {
            return field.previousValue;
        }

        internal override void SetValue<R>(AbstractGameObjectMember<R> field, R value)
        {
            field.previousValue = value;
        }
    }

    public class DrawSelctor : ValueSelctor
    {
        internal override R SelectValue<R>(AbstractGameObjectMember<R> field)
        {
            return field.drawValue;
        }

        internal override void SetValue<R>(AbstractGameObjectMember<R> field, R value)
        {
            field.drawValue = value;
        }
    }

}
