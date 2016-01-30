using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class GameObjectField
    {
        private enum Modes { Simulation, Previous, Draw };
        private static Modes mode = Modes.Simulation;

        public static void SetModeSimulation()
        {
            mode = Modes.Simulation;
        }

        public static void SetModePrevious()
        {
            mode = Modes.Previous;
        }

        public static void SetModeDraw()
        {
            mode = Modes.Draw;
        }

        public static bool IsModeSimulation()
        {
            return mode == Modes.Simulation;
        }

        public static bool IsModePrevious()
        {
            return mode == Modes.Previous;
        }

        public static bool IsModeDraw()
        {
            return mode == Modes.Draw;
        }

        public GameObjectField(GameObject obj)
        {
            obj.Fields.Add(this);
        }

        //TODO: is this the best way?
        public abstract void ApplyMessage(GameObjectUpdate message);

        public abstract GameObjectUpdate ConstructMessage(GameObjectUpdate message);

        public virtual void Interpolate(float smoothing){}
    }
}
