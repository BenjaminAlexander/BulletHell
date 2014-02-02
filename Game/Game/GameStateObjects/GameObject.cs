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
        private Boolean sendUpdate = true;

        public struct State
        {
            public Boolean destroy;
            public State(Boolean destroy)
            {
                this.destroy = destroy;
            }
        }
        private State state = new State();

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

        protected abstract void UpdateSubclass(GameTime gameTime);

        public void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            sendUpdate = false;
            this.UpdateSubclass(gameTime);
            secondsUntilUpdateMessage = secondsUntilUpdateMessage - secondsElapsed;
            if (secondsUntilUpdateMessage <= 0)
            {
                sendUpdate = true;
                secondsUntilUpdateMessage = secondsBetweenUpdateMessage;
            }
        }

        public abstract void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics);

        public int GetTypeID()
        {
            return GameObject.GetTypeID(this.GetType());
        }

        public virtual void UpdateMemberFields(GameObjectUpdate message)
        {
            message.ResetReader();
            if (!(this.GetType() == GameObject.GetType(message.ReadInt()) && this.id == message.ReadInt()))
            {
                throw new Exception("this message does not belong to this object");
            }
            this.state.destroy = message.ReadBoolean();
        }

        public virtual GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message.Append(this.state.destroy);
            return message;
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
            get { return sendUpdate; }
        }

    }
}
