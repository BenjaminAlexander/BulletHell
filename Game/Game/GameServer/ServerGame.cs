using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

namespace MyGame.GameServer
{
    public class ServerGame : Game1
    {
        private static Vector2 worldSize = new Vector2(20000);
        private Lobby lobby;
        private ServerLogic serverLogic = null;

        //TODO: there needs to be a better way to set up game-mode-ish parameters
        public ServerGame(Lobby lobby)
            : base(worldSize)
        {
            this.lobby = lobby;

            lobby.BroadcastTCP(new SetWorldSize(new GameTime(), worldSize));

        }

        protected override void Initialize()
        {
            base.Initialize();
            serverLogic = new ServerLogic(this, lobby, worldSize);
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

            Queue<ServerUpdate> messageQueue = lobby.DequeueAllInboundMessages();
            while (messageQueue.Count > 0)
            {
                ServerUpdate m = messageQueue.Dequeue();
                m.Apply(this);
            }

            serverLogic.Update(secondsElapsed);

            base.Update(gameTime);

            this.GameObjectCollection.ServerUpdate(lobby, gameTime);
            //this.Camera.Update(focus, secondsElapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this.GameObjectCollection.Draw(gameTime, this.GraphicsObject);
        }
    }
}
