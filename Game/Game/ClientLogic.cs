using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using MyGame.IO;
using MyGame.Networking;
namespace MyGame
{
    public class ClientLogic
    {
        LocalPlayerController controller; 
        public ClientLogic(InputManager io, Camera camera)
        {
            controller = new LocalPlayerController(io, camera);
        }

        public void Update(ThreadSafeQueue<GameMessage> outgoingQueue, float secondsElapsed)
        {
            controller.Update(secondsElapsed);
            outgoingQueue.Enqueue(controller.CurrentState.GetStateMessage());
        }
    }
}
