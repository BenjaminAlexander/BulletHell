using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    public abstract class GameObject
    {

        static GameState localGameState = null;
        static Type[] gameObjectTypeArray;
        static Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();
        static int nextId = 0;
        public static int NextID
        {
            get { return nextId++; }
        }

        private int id;
        public int ID
        {
            get { return id; }
        }
        public static GameState LocalGameState
        {
            get { return localGameState; }
            set { localGameState = value; }
        }

        public static void Initialize()
        {
            IEnumerable<Type> types = System.Reflection.Assembly.GetAssembly(typeof(GameObject)).GetTypes().Where(t => t.IsSubclassOf(typeof(GameObject)));
            types = types.OrderBy(t => t.Name);
            gameObjectTypeArray = types.ToArray();
            gameObjectList.Clear();
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

            gameObjectList.Add(this.id, this);
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

        public virtual Boolean IsPhysicalObject
        {
            get { return false; }
        }
    }
}
