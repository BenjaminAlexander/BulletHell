using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameServer;
using MyGame.Engine.GameState.Instants;

namespace MyGame.AIControllers
{
    public class AIManager
    {
        private List<AbstractAIController> controllerList = new List<AbstractAIController>();

        public void AddController(AbstractAIController controller)
        {
            controllerList.Add(controller);
        }

        public void Update(CurrentInstant current, NextInstant next, float seconds)
        {
            foreach (AbstractAIController controller in controllerList.ToArray())
            {
                controller.Update(current, next, seconds);
            }

        }
    }
}
