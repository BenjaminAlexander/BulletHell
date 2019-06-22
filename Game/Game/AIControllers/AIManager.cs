using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameServer;

namespace MyGame.AIControllers
{
    public class AIManager
    {
        private List<AbstractAIController> controllerList = new List<AbstractAIController>();

        public void AddController(AbstractAIController controller)
        {
            controllerList.Add(controller);
        }

        public void Update(float seconds)
        {
            foreach (AbstractAIController controller in controllerList.ToArray())
            {
                controller.Update(seconds);
            }

        }
    }
}
