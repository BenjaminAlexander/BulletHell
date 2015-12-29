using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using MyGame.Networking;
using Microsoft.Xna.Framework;

namespace MyClient
{
    class ClientGame : Game1
    {
        //TODO: there needs to be a better way to set up game-mode-ish parameters
        //TODO: expecting the world size as the first message like this causes a race condition, i think
        private static Vector2 SetWorldSize(ThreadSafeQueue<GameMessage> incomingQueue)
        {
            // Attempt to get the world size.
            GameMessage m = incomingQueue.Dequeue();
            Stack<GameMessage> searchStack = new Stack<GameMessage>();

            while(!(m is SetWorldSize))
            {
                searchStack.Push(m);
                m = incomingQueue.Dequeue();
            }
            return ((SetWorldSize)m).Size;
        }

        public ClientGame(ThreadSafeQueue<GameMessage> outgoingQueue, ThreadSafeQueue<GameMessage> incomingQueue, int playerID)
            : base(outgoingQueue, incomingQueue, playerID, SetWorldSize(incomingQueue))
        {

        }
    }
}
