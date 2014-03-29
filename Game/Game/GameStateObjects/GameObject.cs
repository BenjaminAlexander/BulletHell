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
        private Boolean destroy = false;
        public static ValueSelctor mode = new SimulationSelctor();
        public ValueSelctor Mode
        {
            get { return mode; }
        }


        public List<IGameObjectMember> fields = new List<IGameObjectMember>();
        public void AddField(IGameObjectMember field)
        {
            field.Obj = this;
            fields.Add(field);
        }

        //this is the time between the sending of each update method
        private float secondsBetweenUpdateMessage = (float)((float)(16 * 6) / (float)1000);
        protected virtual float SecondsBetweenUpdateMessage
        {
            get { return secondsBetweenUpdateMessage; }
        }

        //this is the id for the next game object
        static int nextId = 1;
        private static int NextID
        {
            get { return nextId++; }
        }
   
        private int id;
        private long lastUpdateTimeStamp = 0;
        private RollingAverage averageLatency = new RollingAverage(8);
        private float secondsUntilUpdateMessage = 0;
        float currentSmoothing = 0;

        //For clients every GameObject has three states. For the 
        //server, the draw state, previous state and simulation state reference 
        //the same object
        private State simulationState;
        //private State previousState;
        //private State drawState;

        //this allow subclasses to initalize their part of the state
        public T PracticalState<T>() where T : State
        {
            if (Game1.IsServer)
            {
                return (T)simulationState;
            }
            else
            {
                return (T)simulationState;
            }
        }

        protected virtual void InitializeFields()
        {

        }

        //this class descibes the state of an object and all operation that can be performed on the state
        public class State
        {
            protected void AddField(IGameObjectMember field)
            {
                this.obj.AddField(field);
            }

            protected virtual void InitializeFields()
            {

            }

            private GameObject obj;
            protected T GetObject<T>() where T : GameObject
            {
                return (T)obj;
            }            

            public State(GameObject obj)
            {
                this.obj = obj;
                this.InitializeFields();
            }

            //This method defines how the state should be drawn
            public virtual void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics){}

            //This method defines how the state should be updated
            public virtual void UpdateState(float seconds){}

            public virtual void CommonUpdate(float seconds) {}

        }

        public int ID
        {
            get { return id; }
        }

        public Boolean IsDestroyed
        {
            get { return destroy; }
        }

        public void Destroy()
        {
            this.destroy = true;
        }

        //Constructs a game object from a update message.  
        //The GameObject and update message must already match types
        public GameObject(GameObjectUpdate message)
        {
            Game1.AssertIsNotServer();
            //get blank states for simulation and draw
            //this make it so states are of the type of the current subclass
            this.simulationState = this.BlankState(this);
            //this.drawState = this.BlankState(this);
            //this.previousState = this.BlankState(this);
            this.InitializeFields();
            message.ResetReader();

            //check this message if for the current type
            if (this.GetType() != GameObjectTypes.GetType(message.ReadInt()))
            {
                throw new Exception("Message of incorrect type");
            }

            //get ID
            id = message.ReadInt();

            //initialize draw and simulation states
            this.ApplyMessage(message);
            //drawState.ApplyMessage(message);
            //previousState.ApplyMessage(message);
        }

        public GameObject()
        {
            Game1.AsserIsServer();
            this.simulationState = this.BlankState(this);
            this.InitializeFields();
            //this.drawState = this.simulationState;
            //this.previousState = this.simulationState;
            this.id = NextID;
        }

        //updates the game object for both server and client
        public void Update(float secondsElapsed)
        {
            //update states, always update/predict simulation state
            simulationState.UpdateState(secondsElapsed);
            secondsUntilUpdateMessage = secondsUntilUpdateMessage - secondsElapsed;

            if (Game1.IsServer)
            {
                //if I'm the server, draw and simulation are the same
                //update common and server only
                //drawState = simulationState;
                //previousState = simulationState;
                simulationState.CommonUpdate(secondsElapsed);
                //simulationState.ServerUpdate(secondsElapsed);
            }
            else
            {
                //figure out what weight to interpolate with
                float smoothingDecay = secondsElapsed / SecondsBetweenUpdateMessage;
                currentSmoothing -= smoothingDecay;
                if (currentSmoothing < 0)
                    currentSmoothing = 0;

                //if I'm the client, update draw
                //interpolate draw to move it closer to simulation
                //previousState.UpdateState(secondsElapsed);
                //drawState = this.BlankState(this);
                //previousState.Interpolate(previousState, simulationState, this.currentSmoothing, drawState);
                mode = new PreviousSelctor();
                simulationState.UpdateState(secondsElapsed);
                mode = new SimulationSelctor();


                this.Interpolate(this.currentSmoothing);
                //update common
                this.simulationState.CommonUpdate(secondsElapsed);

                if (secondsUntilUpdateMessage < -SecondsBetweenUpdateMessage * 3)
                {
                    this.Destroy();
                }
            }

            //check if its time to send the next message
            
            /*if (secondsUntilUpdateMessage <= 0)
            {
                secondsUntilUpdateMessage = 0;
            }*/
        }

        //draws the object, simply calls draw on draw state
        public void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            mode = new DrawSelctor();
            simulationState.Draw(gameTime, graphics);
            mode = new SimulationSelctor();
        }

        //returns the type id for this objects type
        public int GetTypeID()
        {
            return GameObjectTypes.GetTypeID(this.GetType());
        }

        //sends an update message
        public virtual void SendUpdateMessage(Queue<GameMessage> messageQueue)
        {
            if (Game1.IsServer && secondsUntilUpdateMessage <= 0)
            {
                GameObjectUpdate m = new GameObjectUpdate(this);
                m = this.ConstructMessage(m);
                //Game1.outgoingQue.Enqueue(m);
                messageQueue.Enqueue(m);
                secondsUntilUpdateMessage = SecondsBetweenUpdateMessage;               
            }
        }

        public virtual void ForceSendUpdateMessage(Queue<GameMessage> messageQueue)
        {
            if (Game1.IsServer)
            {
                GameObjectUpdate m = new GameObjectUpdate(this);
                m = this.ConstructMessage(m);
                //Game1.outgoingQue.Enqueue(m);
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
                //previousState = drawState;
                this.SetPrevious();
                this.ApplyMessage(message);
                lastUpdateTimeStamp = currentTimeStamp;

                TimeSpan deltaSpan = new TimeSpan(Game1.CurrentGameTime.TotalGameTime.Ticks - currentTimeStamp);

                averageLatency.AddValue((float)(deltaSpan.TotalSeconds));

                float timeDeviation = (float)(deltaSpan.TotalSeconds) - averageLatency.AverageValue;
                if (timeDeviation > 0)
                {
                    simulationState.UpdateState(timeDeviation);
                }
            }
        }

        //This method returns a blank state.  Child classes override this to allow 
        //the base class to get a state type of the child type.This method must be 
        //overriden for the child class to work
        protected abstract State BlankState(GameObject obj);


        public void ApplyMessage(GameObjectUpdate message)
        {
            message.ResetReader();
            if (!(this.GetType() == GameObjectTypes.GetType(message.ReadInt()) && this.ID == message.ReadInt()))
            {
                throw new Exception("this message does not belong to this object");
            }
            this.destroy = message.ReadBoolean();

            foreach (IGameObjectMember field in this.fields)
            {
                field.ApplyMessage(message);
            }
            message.AssertMessageEnd();
        }

        public GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.destroy);

            foreach (IGameObjectMember field in this.fields)
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
    }
}
