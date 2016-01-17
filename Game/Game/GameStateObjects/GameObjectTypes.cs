using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;
using MyGame.GameClient;

namespace MyGame.GameStateObjects
{
    static class GameObjectTypes
    {
        private static Boolean isInitialized = false;
        private static Type[] gameObjectTypeArray;

        private static void Initialize()
        {
            IEnumerable<Type> types = System.Reflection.Assembly.GetAssembly(typeof(GameObject)).GetTypes().Where(t => t.IsSubclassOf(typeof(GameObject)));
            types = types.OrderBy(t => t.Name);
            gameObjectTypeArray = types.ToArray();

            //check to make sure every game object type has the required constructor
            foreach (Type t in types)
            {
                Type[] constuctorParamsTypes = new Type[1];
                constuctorParamsTypes[0] = typeof(Game1);
                //constuctorParamsTypes[1] = typeof(GameObjectUpdate);

                System.Reflection.ConstructorInfo constructor = t.GetConstructor(constuctorParamsTypes);
                if (constructor == null)
                {
                    throw new Exception("Game object must have constructor GameObject(Game1)");
                    //TODO: its abstract, what to do?
                }

            }
            isInitialized = true;
        }

        public static int GetTypeID(Type t)
        {
            if (!isInitialized)
            {
                Initialize();
            }

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
            if (!isInitialized)
            {
                Initialize();
            }

            return gameObjectTypeArray[id];
        }
    }
}
