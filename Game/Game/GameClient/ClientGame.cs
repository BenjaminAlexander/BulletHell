using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;

namespace MyGame.GameClient
{
    class ClientGame : Game1
    {
        private ThreadSafeQueue<GameMessage> incomingQueue;
        private ThreadSafeQueue<GameMessage> outgoingQueue;
        private ClientLogic clientLogic = null;

        //TODO: there needs to be a better way to set up game-mode-ish parameters
        //TODO: expecting the world size as the first message like this causes a race condition, i think
        private static Vector2 SetWorldSize(ThreadSafeQueue<GameMessage> incomingQueue)
        {
            // Attempt to get the world size.
            GameMessage m = incomingQueue.Dequeue();
            Stack<GameMessage> searchStack = new Stack<GameMessage>();

            while (!(m is SetWorldSize))
            {
                searchStack.Push(m);
                m = incomingQueue.Dequeue();
            }
            return ((SetWorldSize)m).Size;
        }

        public ClientGame(ThreadSafeQueue<GameMessage> outgoingQueue, ThreadSafeQueue<GameMessage> incomingQueue, int playerID)
            : base(playerID, SetWorldSize(incomingQueue))
        {
            this.incomingQueue = incomingQueue;
            this.outgoingQueue = outgoingQueue;
        }

        protected override void Initialize()
        {
            base.Initialize();
            clientLogic = new ClientLogic(this.InputManager, this.Camera);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Queue<GameMessage> messageQueue = incomingQueue.DequeueAll();
            while (messageQueue.Count > 0)
            {
                GameMessage m = messageQueue.Dequeue();
                if (m is GameUpdate)
                {
                    ((GameUpdate)m).Apply(this);
                }
            }

            clientLogic.Update(outgoingQueue, secondsElapsed);
            base.Update(gameTime);

            StaticGameObjectCollection.Collection.ClientUpdate(gameTime);
            this.Camera.Update(false, secondsElapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}