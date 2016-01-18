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
        private RollingAverage averageLatency = new RollingAverage(8);  //TODO: this latency compensation is half baked
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

        public int TypeID
        {
            get { return GameObjectTypes.GetTypeID(this.GetType()); }
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

        public void ClientInitialize(GameObjectUpdate message)
        {
            message.ResetReader();

            //check this message if for the current type
            if (this.GetType() != GameObjectTypes.GetType(message.ReadInt()))
            {
                throw new Exception("Message of incorrect type");
            }

            //get ID
            id = message.ReadInt();

            this.ApplyMessage(message);

            for (int i = 0; i < this.fields.Count; i++)
            {
                this.fields[i].SetAllToSimulation();
            }
        }
    
        //sends an update message
        public virtual void SendUpdateMessage(Queue<GameMessage> messageQueue, GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            secondsUntilUpdateMessage = secondsUntilUpdateMessage - secondsElapsed;
            if (this.IsDestroyed || secondsUntilUpdateMessage <= 0)
            {
                GameObjectUpdate message = new GameObjectUpdate(gameTime, this);

                message.Append(this.destroy);

                foreach (GameObjectField field in this.fields)
                {
                    message = field.ConstructMessage(message);
                }

                messageQueue.Enqueue(message);
                secondsUntilUpdateMessage = SecondsBetweenUpdateMessage;
            }
        }

        //passes the message to the simulation state
        //more importantly it resets currentsmoothing
        public void UpdateMemberFields(GameObjectUpdate message, GameTime gameTime)
        {
            long currentTimeStamp = message.TimeStamp();
            if (lastUpdateTimeStamp <= currentTimeStamp)
            {
                for (int i = 0; i < this.fields.Count; i++)
                {
                    this.fields[i].SetPrevious();
                }

                this.ApplyMessage(message);
                lastUpdateTimeStamp = currentTimeStamp;
                
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

        //updates the simulation state
        public void ApplyMessage(GameObjectUpdate message)
        {
            this.secondsUntilUpdateMessage = this.SecondsBetweenUpdateMessage;

            message.ResetReader();
            if (!(this.GetType() == GameObjectTypes.GetType(message.ReadInt()) && this.ID == message.ReadInt()))
            {
                throw new Exception("this message does not belong to this object");
            }
            this.destroy = message.ReadBoolean();

            foreach (GameObjectField field in this.fields)
            {
                field.ApplyMessage(message);
            }
            message.AssertMessageEnd();
        }

        public void ClientUpdate(float secondsElapsed)
        {
            secondsUntilUpdateMessage = secondsUntilUpdateMessage - secondsElapsed;
            if (secondsUntilUpdateMessage < -SecondsBetweenUpdateMessage * 1.5)
            {
                this.Destroy();
                return;
            }
            
            float currentSmoothing = secondsUntilUpdateMessage / SecondsBetweenUpdateMessage;

            if (currentSmoothing < 0)
            {
                currentSmoothing = 0;
            }

            //When smoothing = 0, all the weight is on simulation
            for (int i = 0; i < this.fields.Count; i++)
            {
                this.fields[i].Interpolate(currentSmoothing);
            }
        }

        //Update methods are called in the order of ServerOnly, Subclass, SimulationOnly
        public virtual void ServerOnlyUpdate(float secondsElapsed) { }

        public virtual void SubclassUpdate(float secondsElapsed) { }

        public virtual void SimulationStateOnlyUpdate(float secondsElapsed) { }

        public virtual void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics) { }
    }
}
