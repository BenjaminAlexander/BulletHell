using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Networking;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects
{
    public abstract class GameObject
    {
        private int id;                     
        private Boolean destroy = false;
        private Game1 game;
        public List<GameObjectField> fields = new List<GameObjectField>();

        //this is the time between the sending of each update method
        private float secondsBetweenUpdateMessage = (float)((float)(100) / (float)1000);
        private long lastUpdateTimeStamp = 0;
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

        internal long LastUpdateTimeStamp
        {
            get { return lastUpdateTimeStamp; }
            set { lastUpdateTimeStamp = value; }
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

        public void AddField(GameObjectField field)
        {
            fields.Add(field);
        }
    
        //sends an update message
        public virtual void SendUpdateMessage(Lobby lobby, GameTime gameTime)
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

        public void LatencyAdjustment(GameTime gameTime, GameObjectUpdate message)
        {
            this.secondsUntilUpdateMessage = this.SecondsBetweenUpdateMessage;
            this.lastUpdateTimeStamp = message.TimeStamp;

            TimeSpan deltaSpan = new TimeSpan(gameTime.TotalGameTime.Ticks - this.lastUpdateTimeStamp);

            float timeDeviation = (float)(deltaSpan.TotalSeconds) - averageLatency.AverageValue;
            averageLatency.AddValue((float)(deltaSpan.TotalSeconds));
            if (timeDeviation > 0)
            {
                GameObjectField.SetModeSimulation();
                this.SubclassUpdate(timeDeviation);
                this.SimulationStateOnlyUpdate(timeDeviation);

                GameObjectField.SetModePrevious();
                this.SubclassUpdate(timeDeviation);

                GameObjectField.SetModeSimulation();
                this.ClientUpdate(timeDeviation);
            }
        }

        public void ClientUpdate(float secondsElapsed)
        {
            this.secondsUntilUpdateMessage = this.secondsUntilUpdateMessage - secondsElapsed;
            if (this.secondsUntilUpdateMessage < -this.SecondsBetweenUpdateMessage * 10)
            {
                this.Destroy();
                return;
            }

            float currentSmoothing = this.secondsUntilUpdateMessage / this.SecondsBetweenUpdateMessage;

            if (currentSmoothing < 0)
            {
                currentSmoothing = 0;
            }

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
