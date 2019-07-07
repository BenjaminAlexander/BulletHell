using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.PlayerControllers;
using MyGame.Utils;
using MyGame.GameServer;
using System.Net;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameClient
{
    public class ClientGame : Game1
    {
        private ServerConnection serverConnection;

        public ClientGame(IPAddress serverAddress)
            : base()
        {
            this.serverConnection = new ServerConnection(serverAddress, this);

            SetWorldSize m = serverConnection.GetSetWorldSize();
            this.SetWorldSize(m.WorldSize);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public Ship GetLocalPlayerFocus(CurrentInstant current)
        {
            Ship focus = null;
            ControllerFocusObject controllerFocus = this.GameObjectCollection.ControllerFocusObject;
            if (controllerFocus != null)
            {
                focus = controllerFocus.GetFocus(current, serverConnection.Id);
            }
            return focus;
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            this.serverConnection.UpdateControlState(MyGame.GameStateObjects.DataStuctures.GameObjectCollection.SingleInstant.AsCurrent, gameTime);

            //haddle all available messages.  this is done again after the gameObject updates but before draw
            Queue<GameObjectUpdate> messageQueue = this.serverConnection.DequeueAllIncomingUDP();
            while (messageQueue.Count > 0)
            {
                GameObjectUpdate m = messageQueue.Dequeue();
                m.Apply(this, gameTime);
            }

            base.Update(gameTime);
            this.GameObjectCollection.ClientUpdate(gameTime);

            Ship focus = this.GetLocalPlayerFocus(MyGame.GameStateObjects.DataStuctures.GameObjectCollection.SingleInstant.AsCurrent);
            this.Camera.Update(MyGame.GameStateObjects.DataStuctures.GameObjectCollection.SingleInstant.AsCurrent, focus, secondsElapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.GraphicsObject.Begin(Matrix.Identity);

            Ship focus = this.GetLocalPlayerFocus(GameStateObjects.DataStuctures.GameObjectCollection.SingleInstant.AsCurrent);

            this.GraphicsObject.End();
            
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            serverConnection.Disconnect();
        }
    }
}