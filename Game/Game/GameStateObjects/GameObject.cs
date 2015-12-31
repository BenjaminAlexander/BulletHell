using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class GameObject : IUpdateable, IDrawable
    {
        protected static ValueSelctor mode = new SimulationSelctor();

        static int nextId = 1;
        private static int NextID
        {
            get { return nextId++; }
        }



        private int id;
        private Boolean destroy = false;
        public List<IGameObjectField> fields = new List<IGameObjectField>();
        private long lastUpdateTimeStamp = 0;
        private RollingAverage averageLatency = new RollingAverage(8);
        private float secondsUntilUpdateMessage = 0;
        private float currentSmoothing = 0;
        private Game1 game;

        public Game1 Game
        {
            get { return game; } 
        }

        //this is the time between the sending of each update method
        private float secondsBetweenUpdateMessage = (float)((float)(16 * 6) / (float)1000);
        protected virtual float SecondsBetweenUpdateMessage
        {
            get { return secondsBetweenUpdateMessage; }
        }     

        protected virtual void InitializeFields(){}

        //Constructs a game object from a update message.  
        //The GameObject and update message must already match types
        public GameObject(Game1 game, GameObjectUpdate message)
        {
            this.game = game;
            Game1.AssertIsNotServer();

            this.InitializeFields();
            message.ResetReader();

            //check this message if for the current type
            if (this.GetType() != GameObjectTypes.GetType(message.ReadInt()))
            {
                throw new Exception("Message of incorrect type");
            }

            //get ID
            id = message.ReadInt();

            this.ApplyMessage(message);
            this.SetPrevious();
            this.Interpolate(0);
        }

        public GameObject(Game1 game)
        {
            this.game = game;
            Game1.AsserIsServer();
            this.InitializeFields();
            this.id = NextID;
        }

        //updates the game object for both server and client
        public void Update(float secondsElapsed)
        {
            secondsUntilUpdateMessage = secondsUntilUpdateMessage - secondsElapsed;
            if (!this.Game.IsGameServer && secondsUntilUpdateMessage < -SecondsBetweenUpdateMessage * 1.5)
            {
                this.Destroy();
                return;
            }

            //update states, always update/predict simulation state
            this.SubclassUpdate(secondsElapsed);
            this.SimulationStateOnlyUpdate(secondsElapsed);

            if (!this.Game.IsGameServer)
            {
                //figure out what weight to interpolate with
                float smoothingDecay = secondsElapsed / SecondsBetweenUpdateMessage;
                currentSmoothing -= smoothingDecay;
                if (currentSmoothing < 0)
                    currentSmoothing = 0;

                mode = new PreviousSelctor();
                this.SubclassUpdate(secondsElapsed);
                mode = new SimulationSelctor();

                this.Interpolate(this.currentSmoothing);
            }
        }

        //draws the object, simply calls draw on draw state
        public void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            mode = new DrawSelctor();
            this.DrawSub(gameTime, graphics);
            mode = new SimulationSelctor();
        }
    
        //sends an update message
        public virtual void SendUpdateMessage(Queue<GameMessage> messageQueue)
        {
            if (secondsUntilUpdateMessage <= 0)
            {
                ForceSendUpdateMessage(messageQueue);
            }
        }

        public virtual void ForceSendUpdateMessage(Queue<GameMessage> messageQueue)
        {
            if (this.Game.IsGameServer)
            {
                GameObjectUpdate m = new GameObjectUpdate(this.Game.CurrentGameTime,  this);
                m = this.ConstructMessage(m);
                messageQueue.Enqueue(m);
                secondsUntilUpdateMessage = SecondsBetweenUpdateMessage;
            }
        }

        //passes the message to the simulation state
        //more importantly it resets currentsmoothing
        public void UpdateMemberFields(GameObjectUpdate message)
        {
            long currentTimeStamp = message.TimeStamp();
            if (lastUpdateTimeStamp <= currentTimeStamp)
            {
                secondsUntilUpdateMessage = SecondsBetweenUpdateMessage;
                currentSmoothing = 1;
                this.SetPrevious();
                this.ApplyMessage(message);
                lastUpdateTimeStamp = currentTimeStamp;

                TimeSpan deltaSpan = new TimeSpan(game.CurrentGameTime.TotalGameTime.Ticks - currentTimeStamp);

                averageLatency.AddValue((float)(deltaSpan.TotalSeconds));

                float timeDeviation = (float)(deltaSpan.TotalSeconds) - averageLatency.AverageValue;
                if (timeDeviation > 0)
                {
                    this.SubclassUpdate(timeDeviation);
                    this.SimulationStateOnlyUpdate(timeDeviation);
                }
            }
        }


        public void ApplyMessage(GameObjectUpdate message)
        {
            message.ResetReader();
            if (!(this.GetType() == GameObjectTypes.GetType(message.ReadInt()) && this.ID == message.ReadInt()))
            {
                throw new Exception("this message does not belong to this object");
            }
            this.destroy = message.ReadBoolean();

            foreach (IGameObjectField field in this.fields)
            {
                field.ApplyMessage(message);
            }
            message.AssertMessageEnd();
        }

        public GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.destroy);

            foreach (IGameObjectField field in this.fields)
            {
                message = field.ConstructMessage(message);
            }

            return message;
        }

        //When smoothing = 0, all the weight is on s
        public void Interpolate(float smoothing)
        {
            for (int i = 0; i < this.fields.Count; i++)
            {
                this.fields[i].Interpolate(smoothing);
            }
        }

        public void SetPrevious()
        {
            for (int i = 0; i < this.fields.Count; i++)
            {
                this.fields[i].SetPrevious();
            }
        }

        public virtual void DrawSub(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics) { }

        public virtual void SimulationStateOnlyUpdate(float seconds) { }

        public virtual void SubclassUpdate(float seconds) { }

        public virtual void Destroy()
        {
            this.destroy = true;
        }

        public void AddField(IGameObjectField field)
        {
            fields.Add(field);
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

        public ValueSelctor Mode
        {
            get { return mode; }
        }

        public static ValueSelctor StaticMode
        {
            get { return mode; }
        }
    }
}
