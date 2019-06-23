﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects
{
    public abstract class GameObject : Engine.GameState.GameObject
    {
        protected const float secondsElapsed = .02f;

        private int id;                     
        private Game1 game;

        //this is the time between the sending of each update method
        private float secondsBetweenUpdateMessage = (float)((float)(100) / (float)1000); 
        private float secondsUntilUpdateMessage = 0;

        public Game1 Game
        {
            get { return game; } 
        }

        public int ID
        {
            get { return id; }
            internal set { id = value; }
        }

        protected virtual float SecondsBetweenUpdateMessage
        {
            get { return secondsBetweenUpdateMessage; }
        }

        public GameObject()
        {
        }

        public GameObject(Game1 game)
        {
            this.game = game;
            //TODO: it doesn't seem right to check if the game is a server game
            //but it seems better than having a seperate method for this
            if (game is ServerGame)
            {
                this.id = game.GameObjectCollection.NextID;
                this.SetUp(this.id, new Instant(0));
            }
        }
    
        //sends an update message
        public void SendUpdateMessage(Lobby lobby, GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            this.secondsUntilUpdateMessage = this.secondsUntilUpdateMessage - secondsElapsed;
            if (this.secondsUntilUpdateMessage <= 0)
            {
                GameObjectUpdate message = new GameObjectUpdate(gameTime, this);
                lobby.BroadcastUDP(message);
                this.secondsUntilUpdateMessage = this.SecondsBetweenUpdateMessage;
            }
        }

        //Update methods are called in the order of ServerOnly, Subclass, SimulationOnly

        public virtual void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics) { }
    }
}
