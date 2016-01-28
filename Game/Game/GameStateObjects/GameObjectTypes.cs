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
        private static Type[] gameObjectTypeArray;
        private static Dictionary<Type, System.Reflection.ConstructorInfo> constructorDictionary = new Dictionary<Type,System.Reflection.ConstructorInfo>();

        public static void Initialize()
        {
            //TODO: theres a race condition that causes a crash here
            IEnumerable<Type> types = System.Reflection.Assembly.GetAssembly(typeof(GameObject)).GetTypes().Where(t => t.IsSubclassOf(typeof(GameObject)));
            types = types.OrderBy(t => t.Name);
            gameObjectTypeArray = types.ToArray();

            //check to make sure every game object type has the required constructor
            for (int i = 0; i < gameObjectTypeArray.Length; i++)
            {
                Type[] constuctorParamsTypes = new Type[1];
                constuctorParamsTypes[0] = typeof(Game1);

                System.Reflection.ConstructorInfo constructor = gameObjectTypeArray[i].GetConstructor(constuctorParamsTypes);
                if (constructor == null)
                {
                    throw new Exception("Game object must have constructor GameObject(Game1)");
                }
                constructorDictionary[gameObjectTypeArray[i]] = constructor;
            }
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

        public static GameObject Construct(int typeID, ClientGame game)
        {
            return Construct(gameObjectTypeArray[typeID], game);
        }

        public static GameObject Construct(Type type, ClientGame game)
        {
            object[] constuctorParams = new object[1];
            constuctorParams[0] = game;
            return (GameObject)constructorDictionary[type].Invoke(constuctorParams);
        }
    }
}
