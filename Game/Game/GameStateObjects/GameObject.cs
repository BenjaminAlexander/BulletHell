using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects
{
    public abstract class GameObject
    {
        private int id;                     
        private Boolean destroy = false;
        private Game1 game;
        private List<GameObjectField> fields = new List<GameObjectField>();

        //this is the time between the sending of each update method
        private float secondsBetweenUpdateMessage = (float)((float)(100) / (float)1000);
        private long lastMessageTimeStamp = 0;
        //TODO: this latency compensation is half baked
        //TODO: Make this static?  Differentiate between TCP and UDP messages?  Push this into the message layer
        private RollingAverage averageLatency = new RollingAverage(100);  
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

        public Boolean IsDestroyed
        {
            get { return destroy; }
            internal set { destroy = value; }
        }

        internal List<GameObjectField> Fields
        {
            get
            {
                return fields;
            }
        }

        internal long LastMessageTimeStamp
        {
            get { return lastMessageTimeStamp; }
        }


        protected virtual float SecondsBetweenUpdateMessage
        {
            get { return secondsBetweenUpdateMessage; }
        }

        public GameObject(Game1 game)
        {
            this.game = game;
            //TODO: it doesn't seem right to check if the game is a server game
            //but it seems better than having a seperate method for this
            if (game is ServerGame)
            {
                this.id = game.GameObjectCollection.NextID;
            }
        }

        public virtual void Destroy()
        {
            this.destroy = true;
        }
    
        //sends an update message
        public void SendUpdateMessage(Lobby lobby, GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            this.secondsUntilUpdateMessage = this.secondsUntilUpdateMessage - secondsElapsed;
            if (this.IsDestroyed || this.secondsUntilUpdateMessage <= 0)
            {
                GameObjectUpdate message = new GameObjectUpdate(gameTime, this);
                lobby.BroadcastUDP(message);
                this.secondsUntilUpdateMessage = this.SecondsBetweenUpdateMessage;
            }
        }

        public void ServerUpdate(float secondsElapsed)
        {
            this.ServerOnlyUpdate(secondsElapsed);
            this.SubclassUpdate(secondsElapsed);
            this.SimulationStateOnlyUpdate(secondsElapsed);
        }

        public void ClientUpdate(float secondsElapsed)
        {
            GameObjectField.SetModeSimulation();
            this.SubclassUpdate(secondsElapsed);
            this.SimulationStateOnlyUpdate(secondsElapsed);

            GameObjectField.SetModePrevious();
            this.SubclassUpdate(secondsElapsed);

            GameObjectField.SetModeSimulation();
            this.Smooth(secondsElapsed);
        }

        public virtual void LatencyAdjustment(GameTime gameTime, long messageTimeStamp)
        {
            this.secondsUntilUpdateMessage = this.SecondsBetweenUpdateMessage;
            this.lastMessageTimeStamp = messageTimeStamp;

            TimeSpan deltaSpan = new TimeSpan(gameTime.TotalGameTime.Ticks - this.lastMessageTimeStamp);
            averageLatency.AddValue((float)(deltaSpan.TotalSeconds));
            float timeDeviation = (float)(deltaSpan.TotalSeconds) - averageLatency.AverageValue;
            if (timeDeviation > 0)
            {
                GameObjectField.SetModeSimulation();
                this.SubclassUpdate(timeDeviation);
                this.SimulationStateOnlyUpdate(timeDeviation);
                this.Smooth(timeDeviation);
            }
        }

        private void Smooth(float secondsElapsed)
        {
            this.secondsUntilUpdateMessage = this.secondsUntilUpdateMessage - secondsElapsed;
            if (this.secondsUntilUpdateMessage < -this.SecondsBetweenUpdateMessage * 10)
            {
                this.Destroy();
                return;
            }

            float currentSmoothing = Math.Max(this.secondsUntilUpdateMessage / this.SecondsBetweenUpdateMessage, 0);

            //When smoothing = 0, all the weight is on simulation
            foreach (GameObjectField field in this.fields)
            {
                field.Interpolate(currentSmoothing);
            }
        }

        //Update methods are called in the order of ServerOnly, Subclass, SimulationOnly
        public virtual void ServerOnlyUpdate(float secondsElapsed) { }

        public virtual void SubclassUpdate(float secondsElapsed) { }

        public virtual void SimulationStateOnlyUpdate(float secondsElapsed) { }

        public virtual void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics) { }
    }
}
