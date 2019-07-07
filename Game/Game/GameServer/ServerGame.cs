﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameClient;
using MyGame.AIControllers;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameServer
{
    public class ServerGame : Game1
    {
        private static Vector2 worldSize = new Vector2(20000);
        private Lobby lobby;
        private ServerLogic serverLogic = null;
        private AIManager aiManager = new AIManager();

        public AIManager AIManager
        {
            get
            {
                return aiManager;
            }
        }

        //TODO: there needs to be a better way to set up game-mode-ish parameters
        public ServerGame(Lobby lobby)
            : base()
        {
            this.lobby = lobby;

            lobby.BroadcastTCP(new SetWorldSize(worldSize));
            this.SetWorldSize(worldSize);
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.SetWorldSize(worldSize);
            serverLogic = new ServerLogic(this, lobby, worldSize);
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            lobby.Update();
            aiManager.Update(MyGame.GameStateObjects.DataStuctures.GameObjectCollection.SingleInstant.AsCurrent, MyGame.GameStateObjects.DataStuctures.GameObjectCollection.SingleInstant.AsNext, secondsElapsed);

            base.Update(gameTime);

            this.GameObjectCollection.ServerUpdate(lobby, gameTime);
        }
    }
}
