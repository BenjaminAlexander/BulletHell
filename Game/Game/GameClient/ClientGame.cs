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
using MyGame.GameServer;
using System.Net;

namespace MyGame.GameClient
{
    public class ClientGame : Game1
    {
        private int playerID;
        private ServerConnection serverConnection;
        private LocalPlayerController controller;

        public int PlayerID
        {
            get { return playerID; }
        }

        public ClientGame(IPAddress serverAddress)
            : base()
        {
            this.serverConnection = new ServerConnection(serverAddress);
            this.playerID = serverConnection.Id;

            SetWorldSize m = new SetWorldSize(serverConnection.UdpTcpPair);
            this.SetWorldSize(m.WorldSize);
        }

        protected override void Initialize()
        {
            base.Initialize();
            controller = new LocalPlayerController(this);
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

            controller.Update(secondsElapsed);
            this.serverConnection.SendUDP(controller.GetStateMessage(gameTime));

            //haddle all available messages.  this is done again after the gameObject updates but before draw
            Queue<GameObjectUpdate> messageQueue = this.serverConnection.DequeueAllIncomingUDP();
            while (messageQueue.Count > 0)
            {
                GameObjectUpdate m = messageQueue.Dequeue();
                m.Apply(this, gameTime);
            }

            base.Update(gameTime);
            this.GameObjectCollection.ClientUpdate(gameTime);

            GameObjectField.SetModeDraw();
            Ship focus = this.GetLocalPlayerFocus();
            this.Camera.Update(focus, secondsElapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            GameObjectField.SetModeDraw();
            base.Draw(gameTime);

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

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            serverConnection.Disconnect();
        }
    }
}