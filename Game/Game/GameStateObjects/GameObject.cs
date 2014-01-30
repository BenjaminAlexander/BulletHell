using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public abstract class GameObject : IUpdateable, IDrawable
    {

        static GameState localGameState = null;
        static Type[] gameObjectTypeArray;
        static GameObjectCollection gameObjectCollection;
        static int nextId = 0;

        private int id;
        private Boolean destroy = false;

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
            destroy = true;
        }

        public Boolean IsDestroyed
        {
            get { return destroy; }
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
            gameObjectCollection.Add(this);
        }

        public void Initialize()
        {
            
        }

        GameState gameState = null;

        public GameState GameState
        {
            get { return gameState; }
        }

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

    }
}
