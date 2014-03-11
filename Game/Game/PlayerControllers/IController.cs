using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;

namespace MyGame.PlayerControllers
{
    public interface IController
    {
        PlayerControlState CurrentState
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
