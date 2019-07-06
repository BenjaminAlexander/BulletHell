using System;
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

        //this is the time between the sending of each update method
        private float secondsUntilUpdateMessage = 0;
    
        //sends an update message
        internal void SendUpdateMessage(Lobby lobby, GameTime gameTime, Engine.GameState.GameObjectCollection collection, Instant instant)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            this.secondsUntilUpdateMessage = this.secondsUntilUpdateMessage - secondsElapsed;
            if (this.secondsUntilUpdateMessage <= 0)
            {
                GameObjectUpdate message = new GameObjectUpdate(gameTime, this, collection, instant);
                lobby.BroadcastUDP(message);
                this.secondsUntilUpdateMessage = (float)((float)(100) / (float)1000);
            }
        }

        //Update methods are called in the order of ServerOnly, Subclass, SimulationOnly

        public virtual void Draw(CurrentInstant current, DrawingUtils.MyGraphicsClass graphics) { }
    }
}
