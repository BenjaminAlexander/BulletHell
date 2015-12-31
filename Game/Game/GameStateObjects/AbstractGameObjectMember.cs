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

    abstract class AbstractGameObjectField<T> : IGameObjectField
    {
        internal T simulationValue;
        internal T previousValue;
        internal T drawValue;

        private GameObject obj;

        public AbstractGameObjectField(GameObject obj, T v)
        {
            this.obj = obj;
            simulationValue = v;
            drawValue = v;
            previousValue = v;
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

        public abstract void SetPrevious();
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
