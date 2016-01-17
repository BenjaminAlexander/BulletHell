using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    static class GameObjectFieldMode
    {
        private static ValueSelctor mode = new SimulationSelctor();

        public static ValueSelctor Mode
        {
            get { return mode; }
        }

        public static void SetModeSimulation()
        {
            mode = new SimulationSelctor();
        }

        public static void SetModePrevious()
        {
            mode = new PreviousSelctor();
        }

        public static void SetModeDraw()
        {
            mode = new DrawSelctor();
        }
    }

    public abstract class AbstractGameObjectField<T> : IGameObjectField
    {
        internal T simulationValue;
        internal T previousValue;
        internal T drawValue;

        public AbstractGameObjectField(GameObject obj, T v)
        {
            simulationValue = v;
            drawValue = v;
            previousValue = v;
            obj.AddField(this);
        }

        public static implicit operator T(AbstractGameObjectField<T> m)
        {
            return m.Value;
        }
        
        public T Value
        {
            get 
            {
                ValueSelctor s = GameObjectFieldMode.Mode;
                return s.SelectValue<T>(this); 
            }
            set 
            {
                ValueSelctor s = GameObjectFieldMode.Mode;
                s.SetValue<T>(this, value);
            }
        }

        public abstract void ApplyMessage(GameObjectUpdate message);

        public abstract GameObjectUpdate ConstructMessage(GameObjectUpdate message);

        public abstract void Interpolate(float smoothing);

        public void SetPrevious()
        {
            this.previousValue = this.drawValue;
        }

        public void SetAllToSimulation()
        {
            this.previousValue = this.simulationValue;
            this.drawValue = this.simulationValue;
        }
    }

    public abstract class ValueSelctor
    {
        internal abstract R SelectValue<R>(AbstractGameObjectField<R> field);
        internal abstract void SetValue<R>(AbstractGameObjectField<R> field, R value);
    }

    public class SimulationSelctor : ValueSelctor
    {
        internal override R SelectValue<R>(AbstractGameObjectField<R> field)
        {
            return field.simulationValue;
        }

        internal override void SetValue<R>(AbstractGameObjectField<R> field, R value)
        {
            field.simulationValue = value;
        }
    }

    public class PreviousSelctor : ValueSelctor
    {
        internal override R SelectValue<R>(AbstractGameObjectField<R> field)
        {
            return field.previousValue;
        }

        internal override void SetValue<R>(AbstractGameObjectField<R> field, R value)
        {
            field.previousValue = value;
        }
    }

    public class DrawSelctor : ValueSelctor
    {
        internal override R SelectValue<R>(AbstractGameObjectField<R> field)
        {
            return field.drawValue;
        }

        internal override void SetValue<R>(AbstractGameObjectField<R> field, R value)
        {
            field.drawValue = value;
        }
    }

}
