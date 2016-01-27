using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.PlayerControllers;
using MyGame.Utils;

namespace MyGame.GameClient
{
    public class ClientGame : Game1
    {
        private ThreadSafeQueue<GameMessage> incomingQueue;
        private ThreadSafeQueue<GameMessage> outgoingQueue;
        private int playerID;
        private LocalPlayerController controller;

        public int PlayerID
        {
            get { return playerID; }
        }

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
            : base(SetWorldSize(incomingQueue))
        {
            this.playerID = playerID;
            this.incomingQueue = incomingQueue;
            this.outgoingQueue = outgoingQueue;
        }

        protected override void Initialize()
        {
            base.Initialize();
            controller = new LocalPlayerController(this);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public Ship GetLocalPlayerFocus()
        {
            Ship focus = null;
            List<ControllerFocusObject> controllerFocusList = this.GameObjectCollection.GetMasterList().GetList<ControllerFocusObject>();
            if (controllerFocusList.Count > 0)
            {
                ControllerFocusObject controllerFocus = controllerFocusList[0];
                focus = controllerFocus.GetFocus(this.PlayerID);
            }
            return focus;
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Queue<GameMessage> messageQueue = incomingQueue.DequeueAll();
            while (messageQueue.Count > 0)
            {
                GameMessage m = messageQueue.Dequeue();
                if (m is ClientUpdate)
                {
                    ((ClientUpdate)m).Apply(this, gameTime);
                }
            }

            controller.Update(secondsElapsed);
            outgoingQueue.Enqueue(controller.GetStateMessage(gameTime));

            base.Update(gameTime);
            this.GameObjectCollection.ClientUpdate(gameTime);

            Ship focus = this.GetLocalPlayerFocus();
            this.Camera.Update(focus, secondsElapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GameObjectField.SetModeDraw();
            this.GameObjectCollection.Draw(gameTime, this.GraphicsObject);
            GameObjectField.SetModeSimulation();

            this.GraphicsObject.Begin(Matrix.Identity);

            Ship focus = this.GetLocalPlayerFocus();

            if (focus != null)
            {
                this.GraphicsObject.DrawDebugFont("Health: " + focus.Health.ToString(), new Vector2(0), 1);
                this.GraphicsObject.DrawDebugFont("Kills: " + focus.Kills().ToString(), new Vector2(0, 30), 1);
                this.GraphicsObject.DrawDebugFont("Towers Left: " + this.GameObjectCollection.GetMasterList().GetList<Tower>().Count, new Vector2(0, 60), 1);
            }

            this.GraphicsObject.End();
            
        }
    }
}