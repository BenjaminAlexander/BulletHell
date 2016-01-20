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
        private float secondsBetweenUpdateMessage = (float)((float)(16 * 6) / (float)1000);
        private long lastUpdateTimeStamp = 0;
        //TODO: this latency compensation is half baked
        //TODO: Make this static?  Differentiate between TCP and UDP messages?  Push this into the message layer
        private RollingAverage averageLatency = new RollingAverage(8);  
        private float secondsUntilUpdateMessage = 0;

        public Game1 Game
        {
            get { return game; } 
        }

        public int ID
        {
            get { return id; }
        }

        public Boolean IsDestroyed
        {
            get { return destroy; }
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

        public void ClientInitialize(GameObjectUpdate message, GameTime gameTime)
        {
            message.ResetReader();
            int typeIDFromMessage = message.ReadInt();

            //get ID
            this.id = message.ReadInt();

            this.UpdateMemberFields(message, gameTime);

            for (int i = 0; i < this.fields.Count; i++)
            {
                this.fields[i].SetAllToSimulation();
            }
        }

        protected GameObjectUpdate GetUpdateMessage(GameTime gameTime)
        {
            //TODO: review GameObjectUpdate constructor
            GameObjectUpdate message = new GameObjectUpdate(gameTime, this);
            message.Append(this.destroy);
            foreach (GameObjectField field in this.fields)
            {
                message = field.ConstructMessage(message);
            }
            return message;
        }
    
        //sends an update message
        public virtual void SendUpdateMessage(Lobby lobby, GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            this.secondsUntilUpdateMessage = this.secondsUntilUpdateMessage - secondsElapsed;
            if (this.IsDestroyed || this.secondsUntilUpdateMessage <= 0)
            {
                GameObjectUpdate message = this.GetUpdateMessage(gameTime);
                lobby.BroadcastUDP(message);
                this.secondsUntilUpdateMessage = this.SecondsBetweenUpdateMessage;
            }
        }

        //passes the message to the simulation state
        //more importantly it resets currentsmoothing
        public void UpdateMemberFields(GameObjectUpdate message, GameTime gameTime)
        {
            long currentTimeStamp = message.TimeStamp();
            if (lastUpdateTimeStamp <= currentTimeStamp)
            {
                message.ResetReader();

                //Verify the message is for this object
                Type typeFromMessage = GameObjectTypes.GetType(message.ReadInt());
                int idFromMessage = message.ReadInt();

                if (!(this.GetType() == typeFromMessage && this.ID == idFromMessage))
                {
                    throw new Exception("this message does not belong to this object");
                }

                foreach (GameObjectField field in this.fields)
                {
                    field.SetPrevious();
                }

                this.destroy = message.ReadBoolean();

                foreach (GameObjectField field in this.fields)
                {
                    field.ApplyMessage(message);
                }
                message.AssertMessageEnd();

                this.secondsUntilUpdateMessage = this.SecondsBetweenUpdateMessage;
                this.lastUpdateTimeStamp = currentTimeStamp;
                
                TimeSpan deltaSpan = new TimeSpan(gameTime.TotalGameTime.Ticks - currentTimeStamp);
                averageLatency.AddValue((float)(deltaSpan.TotalSeconds));
                float timeDeviation = (float)(deltaSpan.TotalSeconds) - averageLatency.AverageValue;
                if (timeDeviation > 0)
                {
                    this.SubclassUpdate(timeDeviation);
                    this.SimulationStateOnlyUpdate(timeDeviation);
                }
            }
        }

        public void ClientUpdate(float secondsElapsed)
        {
            this.secondsUntilUpdateMessage = this.secondsUntilUpdateMessage - secondsElapsed;
            if (this.secondsUntilUpdateMessage < -this.SecondsBetweenUpdateMessage * 1.5)
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
