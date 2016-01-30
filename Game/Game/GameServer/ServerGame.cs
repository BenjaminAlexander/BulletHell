using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameClient;

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
            lobby.Update();
            serverLogic.Update(secondsElapsed);

            base.Update(gameTime);

            this.GameObjectCollection.ServerUpdate(lobby, gameTime);


            //
            List<BigShip> list = this.GameObjectCollection.GetMasterList().GetList<BigShip>();
            if (list.Count != 0)
            {
                this.Camera.Update(list[0], secondsElapsed);
            }
            //
            
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this.GameObjectCollection.Draw(gameTime, this.GraphicsObject);
        }
    }
}
