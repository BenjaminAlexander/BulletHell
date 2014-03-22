using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using MyGame.IO;
namespace MyGame
{
    class ClientLogic
    {
        LocalPlayerController controller; 
        public ClientLogic(InputManager io, Camera camera)
        {
            controller = new LocalPlayerController(io, camera);
        }

        public void Update(float secondsElapsed)
        {
            if (!Game1.IsServer)
            {
                controller.Update(secondsElapsed);
                Game1.outgoingQueue.Enqueue(controller.CurrentState.GetStateMessage());
            }
        }
    }
}
