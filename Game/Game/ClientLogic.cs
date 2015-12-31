using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using MyGame.IO;
using MyGame.Networking;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public class ClientLogic
    {
        LocalPlayerController controller; 
        public ClientLogic(int playerID, InputManager io, Camera camera)
        {
            controller = new LocalPlayerController(playerID, io, camera);
        }

        public void Update(ThreadSafeQueue<GameMessage> outgoingQueue, float secondsElapsed, GameTime currentGameTime)
        {
            controller.Update(secondsElapsed);
            outgoingQueue.Enqueue(controller.CurrentState.GetStateMessage(currentGameTime));
        }
    }
}
