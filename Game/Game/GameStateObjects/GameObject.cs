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
        //TODO: this is for creating new gameObject IDs.  This should probably only be done on the server
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

        public virtual void ClientInitialize(ClientGame game, GameObjectUpdate message)
        {
            this.game = game;

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

        protected virtual void GameObjectInit(ServerGame game)
        {
            this.game = game;
            this.InitializeFields();
            this.id = NextID;
        }

        public void UpdateSecondsUntilMessage(float secondsElapsed)
        {
            secondsUntilUpdateMessage = secondsUntilUpdateMessage - secondsElapsed;
        }

        public void ClientUpdateTimeout(float secondsElapsed)
        {
            this.UpdateSecondsUntilMessage(secondsElapsed);
            if (secondsUntilUpdateMessage < -SecondsBetweenUpdateMessage * 1.5)
            {
                this.Destroy();
            }
        }
    
        //sends an update message
        public virtual void SendUpdateMessage(Queue<GameMessage> messageQueue, GameTime gameTime)
        {
            if (secondsUntilUpdateMessage <= 0)
            {
                ForceSendUpdateMessage(messageQueue, gameTime);
            }
        }

        public virtual void ForceSendUpdateMessage(Queue<GameMessage> messageQueue, GameTime gameTime)
        {
            GameObjectUpdate m = new GameObjectUpdate(gameTime, this);
            m = this.ConstructMessage(m);
            messageQueue.Enqueue(m);
            secondsUntilUpdateMessage = SecondsBetweenUpdateMessage;
        }

        //passes the message to the simulation state
        //more importantly it resets currentsmoothing
        public void UpdateMemberFields(GameObjectUpdate message, GameTime gameTime)
        {
            long currentTimeStamp = message.TimeStamp();
            if (lastUpdateTimeStamp <= currentTimeStamp)
            {
                secondsUntilUpdateMessage = SecondsBetweenUpdateMessage;
                currentSmoothing = 1;
                this.SetPrevious();
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

        public void UpdateInterpolation(float secondsElapsed)
        {
            //figure out what weight to interpolate with
            float smoothingDecay = secondsElapsed / SecondsBetweenUpdateMessage;
            currentSmoothing -= smoothingDecay;
            if (currentSmoothing < 0)
            {
                currentSmoothing = 0;
            }

            this.Interpolate(this.currentSmoothing);
        }

        //When smoothing = 0, all the weight is on s
        private void Interpolate(float smoothing)
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

        public virtual void ServerOnlyUpdate(float seconds) { }

        public virtual void Destroy()
        {
            this.destroy = true;
        }

        public IntegerGameObjectMember AddIntegerGameObjectMember(int value)
        {
            IntegerGameObjectMember field = new IntegerGameObjectMember(this, value);
            fields.Add(field);
            return field;
        }

        public FloatGameObjectMember AddFloatGameObjectMember(float value)
        {
            FloatGameObjectMember field = new FloatGameObjectMember(this, value);
            fields.Add(field);
            return field;
        }
  
        public GameObjectReferenceListField<T> AddGameObjectReferenceListField<T>() where T : GameObject
        {
            GameObjectReferenceListField<T> field = new GameObjectReferenceListField<T>(this, new List<GameObjectReference<T>>(), this.Game.GameObjectCollection);
            fields.Add(field);
            return field;
        }

        public InterpolatedAngleGameObjectMember AddInterpolatedAngleGameObjectMember(float value)
        {
            InterpolatedAngleGameObjectMember field = new InterpolatedAngleGameObjectMember(this, value);
            fields.Add(field);
            return field;
        }

        public Vector2GameObjectMember AddVector2GameObjectMember(Vector2 value)
        {
            Vector2GameObjectMember field = new Vector2GameObjectMember(this, value);
            fields.Add(field);
            return field;
        }

        public GameObjectReferenceField<T> AddGameObjectReferenceField<T>(GameObjectReference<T> value) where T : GameObject
        {
            GameObjectReferenceField<T> field = new GameObjectReferenceField<T>(this, value, this.Game.GameObjectCollection);
            fields.Add(field);
            return field;
        }

        public InterpolatedVector2GameObjectMember AddInterpolatedVector2GameObjectMember(Vector2 value)
        {
            InterpolatedVector2GameObjectMember field = new InterpolatedVector2GameObjectMember(this, value);
            fields.Add(field);
            return field;
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
    }
}
