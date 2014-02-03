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
        static Type[] gameObjectTypeArray;
        static GameObjectCollection gameObjectCollection;
        static int nextId = 1;
        static float secondsBetweenUpdateMessage = (float)((float)(16 * 6) / (float)1000);

        public static int NextID
        {
            get { return nextId++; }
        }

        public static GameObjectCollection Collection
        {
            get { return gameObjectCollection; }
        }

        public static void InitializeGameObjects(Vector2 worldSize)
        {
            IEnumerable<Type> types = System.Reflection.Assembly.GetAssembly(typeof(GameObject)).GetTypes().Where(t => t.IsSubclassOf(typeof(GameObject)));
            types = types.OrderBy(t => t.Name);
            gameObjectTypeArray = types.ToArray();

            //check to make sure every game object type has the required constructor
            foreach (Type t in types)
            {
                Type[] constuctorParamsTypes = new Type[1];
                constuctorParamsTypes[0] = typeof(int);

                System.Reflection.ConstructorInfo constructor = t.GetConstructor(constuctorParamsTypes);
                if (constructor == null)
                {
                    throw new Exception("Game object must have constructor GameObject(int)");
                }
            }
            gameObjectCollection = new GameObjectCollection(worldSize);
        }

        public static int GetTypeID(Type t)
        {
            if (!t.IsSubclassOf(typeof(GameObject)))
            {
                throw new Exception("Not a type of GameObject");
            }

            for (int i = 0; i < gameObjectTypeArray.Length; i++)
            {
                if (gameObjectTypeArray[i] == t)
                {
                    return i;
                }
            }
            throw new Exception("Unknown type of GameObject");
        }

        public static Type GetType(int id)
        {
            return gameObjectTypeArray[id];
        }

        private int id;
        private float secondsUntilUpdateMessage = 0;
        float currentSmoothing = 0;

        private Boolean sendUpdate = true;

        public class State
        {
            public Boolean destroy = false;

            public virtual void ApplyMessage(GameObjectUpdate message)
            {
                this.destroy = message.ReadBoolean();
            }

            public virtual GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message.Append(this.destroy);
                return message;
            }
        }

        public State state = new State();
        public State drawState = new State();

        public int ID
        {
            get { return id; }
        }

        public virtual void Destroy()
        {
            if (Game1.IsServer)
            {
                state.destroy = true;
            }
        }

        public Boolean IsDestroyed
        {
            get { return state.destroy; }
        }

        public GameObject(int id)
        {
            this.id = id;
        }

        public GameObject()
        {
            this.id = NextID;
        }

        public void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            sendUpdate = false;

            if (!Game1.IsServer)
            {
                float smoothingDecay = secondsElapsed / secondsBetweenUpdateMessage;

                currentSmoothing -= smoothingDecay;

                if (currentSmoothing < 0)
                    currentSmoothing = 0;
            }

            //update states
            this.UpdateState(state, secondsElapsed);

            if (Game1.IsServer)
            {
                drawState = state;
                //myDrawState = myState;
            }
            else
            {
                this.UpdateState(drawState, secondsElapsed);
                this.Interpolate(drawState, state, this.CurrentSmoothing);
            }



            secondsUntilUpdateMessage = secondsUntilUpdateMessage - secondsElapsed;
            if (secondsUntilUpdateMessage <= 0)
            {
                sendUpdate = true;
                secondsUntilUpdateMessage = secondsBetweenUpdateMessage;
            }
        }

        public abstract void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, State s);

        public void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            this.Draw(gameTime, graphics, drawState);
        }

        public int GetTypeID()
        {
            return GameObject.GetTypeID(this.GetType());
        }

        public GameObjectUpdate GetUpdateMessage()
        {
            GameObjectUpdate m = new GameObjectUpdate(this);
            return this.MemberFieldUpdateMessage(m);
        }

        public virtual void SendUpdateMessage()
        {
            if (Game1.IsServer)
            {
                Game1.outgoingQue.Enqueue(this.GetUpdateMessage());
            }
        }

        public Boolean SendUpdate
        {
            get { return sendUpdate && Game1.IsServer; }
        }

        public float UpdateDelay
        {
            get { return secondsBetweenUpdateMessage; }
        }

        public float CurrentSmoothing
        {
            get { return currentSmoothing; }
        }

        protected virtual void UpdateState(State s, float seconds)
        {

        }

        protected virtual void Interpolate(State d, State s, float smoothing)
        {

        }

        public void UpdateMemberFields(GameObjectUpdate message)
        {
            currentSmoothing = 1;

            message.ResetReader();
            if (!(this.GetType() == GameObject.GetType(message.ReadInt()) && this.id == message.ReadInt()))
            {
                throw new Exception("this message does not belong to this object");
            }

            state.ApplyMessage(message);
        }

        public virtual GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            return state.ConstructMessage(message);
        }


    }
}
