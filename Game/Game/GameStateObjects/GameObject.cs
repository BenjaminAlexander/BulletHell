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
        static GameState localGameState = null;
        static Type[] gameObjectTypeArray;
        static GameObjectCollection gameObjectCollection;
        static int nextId = 1;

        private int id;
        public struct State
        {
            public Boolean destroy;
            public State(Boolean destroy)
            {
                this.destroy = destroy;
            }
        }
        private State state = new State();

        public static int NextID
        {
            get { return nextId++; }
        }

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
            get { return state.destroy ; }
        }

        public static GameState LocalGameState
        {
            get { return localGameState; }
            set { localGameState = value; }
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

        public GameObject(int id)
        {
            if (localGameState == null)
            {
                throw new Exception("No Game State");
            }
            this.gameState = localGameState;
            this.id = id;
        }

        public GameObject()
        {
            if (localGameState == null)
            {
                throw new Exception("No Game State");
            }
            this.gameState = localGameState;
            this.id = NextID;
        }

        GameState gameState = null;



        protected abstract void UpdateSubclass(GameTime gameTime);

        public void Update(GameTime gameTime)
        {
            if (gameState != null)
            {
                this.UpdateSubclass(gameTime);
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
            if (this.GetType() == GameObject.GetType(message.ReadInt()) && this.id == message.ReadInt())
            {

            }
            else
            {
                throw new Exception("this message does not belong to this object");
            }
            this.state.destroy = message.ReadBoolean();
            if (this.state.destroy)
            {
                int i;
            }

        }

        public virtual GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            //message.Append(this.GetTypeID());
            //message.Append(this.id);
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

    }
}
