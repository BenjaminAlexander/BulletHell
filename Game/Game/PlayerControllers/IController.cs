using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

namespace MyGame.PlayerControllers
{
    public interface IController
    {
        ControlState CurrentState
        {
            get;
        }

        void Update(float secondsElapsed);

        Ship Focus
        {
            set;
            get;
        }
    }
}
