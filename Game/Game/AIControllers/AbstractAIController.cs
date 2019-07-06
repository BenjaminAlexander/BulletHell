using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameServer;
using MyGame.Engine.GameState.Instants;

namespace MyGame.AIControllers
{
    public abstract class AbstractAIController : ControlState
    {
        private Ship focus = null;

        public Ship Focus
        {
            get
            {
                return focus;
            }
            set
            {
                focus = value;
            }
        }

        public AbstractAIController(ServerGame game)
        {
            game.AIManager.AddController(this);
        }

        public abstract void Update(CurrentInstant current, NextInstant next, float secondsElapsed);
    }
}
