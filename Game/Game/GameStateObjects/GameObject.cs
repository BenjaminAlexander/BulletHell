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
        //this is the time between the sending of each update method
        static float secondsBetweenUpdateMessage = (float)((float)(16 * 6) / (float)1000);

        //this is the id for the next game object
        static int nextId = 1;
        private static int NextID
        {
            get { return nextId++; }
        }
   
        private int id;
        private float secondsUntilUpdateMessage = 0;
        float currentSmoothing = 0;

        //For clients every GameObject has two states. For the 
        //server, the draw state and simulation state refrence 
        //the same object
        private State simulationState;
        private State drawState;

        //these allow subclasses to initalize their part of the state
        protected State SimulationState
        {
            get { return simulationState; }
        }
        protected State DrawState
        {
            get { return drawState; }
        }

        //this class descibes the state of an object and all operation that can be performed on the state
        public class State
        {
            private GameObject obj;

            //NOTE ON DESTROY: Never make destroy modifiably outside this game object.  
            //Only this object can destroy itself because it must also send a message 
            //to clients allowing them to destroy the object as well.  Have this object 
            //check its own properties (i.e. myHealth == 0) and then deside to destroy 
            //itself
            //TODO: Incase the packet to destroy an object is droped, the collection 
            //should periodically send a list of active game objects so that clients 
            //can clean up trash
            private Boolean destroy = false;

            public State(GameObject obj)
            {
                this.obj = obj;
            }

            //This method puts a states members into a message
            public virtual void ApplyMessage(GameObjectUpdate message)
            {
                message.ResetReader();
                if (!(obj.GetType() == GameObjectTypes.GetType(message.ReadInt()) && obj.ID == message.ReadInt()))
                {
                    throw new Exception("this message does not belong to this object");
                }
                this.destroy = message.ReadBoolean();
            }

            //This method sets a states members from a message
            public virtual GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message.Append(this.destroy);
                return message;
            }

            //This method defines how the state should be drawn
            public virtual void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics){}

            //This method defines how the state should be updated
            public virtual void UpdateState(float seconds){}

            //this method defines how to interpolate between two states.  
            //When smoothing = 0, all the weight is on s
            public virtual void Interpolate(State s, float smoothing){}

            //this method defines game logic that should only be run by the server
            public virtual void ServerUpdate(float seconds){}

            //this method defines game logic that should be run 
            //by both server and clients.  It should not matter 
            //if results of this method are slightly inaccurate.
            //Uses may include displaying visual or sound effects.
            //This method differes from update because this method is not called on both
            public virtual void CommonUpdate(float seconds) {}

            //set destroy flag to true
            public virtual void Destroy()
            {
                if (Game1.IsServer)
                {
                    this.destroy = true;
                }
            }

            public Boolean IsDestroyed
            {
                get { return destroy; }
            }
        }

        public int ID
        {
            get { return id; }
        }

        public Boolean IsDestroyed
        {
            get { return simulationState.IsDestroyed; }
        }

        //Constructs a game object from a update message.  
        //The GameObject and update message must already match types
        public GameObject(GameObjectUpdate message)
        {
            //get blank states for simulation and draw
            //this make it so states are of the type of the current subclass
            this.simulationState = this.BlankState(this);
            this.drawState = this.BlankState(this);

            message.ResetReader();

            //check this message if for the current type
            if (this.GetType() != GameObjectTypes.GetType(message.ReadInt()))
            {
                throw new Exception("Message of incorrect type");
            }

            //get ID
            id = message.ReadInt();

            //initialize draw and simulation states
            simulationState.ApplyMessage(message);
            drawState.ApplyMessage(message);
        }

        public GameObject()
        {
            this.simulationState = this.BlankState(this);
            this.drawState = this.BlankState(this);
            this.id = NextID;
        }

        //updates the game object for both server and client
        public void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (!Game1.IsServer)
            {
                float smoothingDecay = secondsElapsed / secondsBetweenUpdateMessage;
                currentSmoothing -= smoothingDecay;
                if (currentSmoothing < 0)
                    currentSmoothing = 0;
            }

            //update states, always update/predict simulation state
            simulationState.UpdateState(secondsElapsed);

            if (Game1.IsServer)
            {
                //if I'm the server, draw and simulation are the same
                //update common and server only
                drawState = simulationState;
                simulationState.CommonUpdate(secondsElapsed);
                simulationState.ServerUpdate(secondsElapsed);
            }
            else
            {
                //if I'm the client, update draw
                //interpolate draw to move it closer to simulation
                drawState.UpdateState(secondsElapsed);
                drawState.Interpolate(simulationState, this.currentSmoothing);

                //update common
                simulationState.CommonUpdate(secondsElapsed);
            }

            //check if its time to send the next message
            secondsUntilUpdateMessage = secondsUntilUpdateMessage - secondsElapsed;
            if (secondsUntilUpdateMessage <= 0)
            {
                secondsUntilUpdateMessage = 0;
            }
        }

        //draws the object, simply calls draw on draw state
        public void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            drawState.Draw(gameTime, graphics);
        }

        //returns the type id for this objects type
        public int GetTypeID()
        {
            return GameObjectTypes.GetTypeID(this.GetType());
        }

        //sends an update message
        public virtual void SendUpdateMessage()
        {
            if (Game1.IsServer && secondsUntilUpdateMessage <= 0)
            {
                GameObjectUpdate m = new GameObjectUpdate(this);
                m = simulationState.ConstructMessage(m);
                Game1.outgoingQue.Enqueue(m);
                secondsUntilUpdateMessage = secondsBetweenUpdateMessage;
            }
        }

        //passes the message to the simulation state
        //more importantly it resets currentsmoothing
        public void UpdateMemberFields(GameObjectUpdate message)
        {
            currentSmoothing = 1;
            simulationState.ApplyMessage(message);
        }

        //This method returns a blank state.  Child classes override this to allow 
        //the base class to get a state type of the child type.This method must be 
        //overriden for the child class to work
        protected virtual State BlankState(GameObject obj)
        {
            return new State(obj);
        }


    }
}
