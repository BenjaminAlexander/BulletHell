using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState;
using EngineTest.EngineTest.TestUtils;
using MyGame.Engine.GameState.Instants;
using Microsoft.Xna.Framework;
using MyGame.Engine.Utils;

namespace EngineTest.EngineTest.GameStateTest
{
    [TestClass]
    public class GameObjectCollectionTest
    {
        GameObjectCollection server;
        GameObjectCollection client;

        SimpleObjectA gameObjectA;
        SimpleObjectB gameObjectB;

        int idA;
        int idB;
        Instant instantA;
        Instant instantB;

        [TestInitialize]
        public void TestInitialize()
        {
            Logger.StartLogger();

            GameObject.AddType<SimpleObjectA>();
            GameObject.AddType<SimpleObjectB>();

            instantA = new Instant(123);
            instantB = new Instant(65);

            server = new GameObjectCollection();
            client = new GameObjectCollection();

            gameObjectA = server.NewGameObject<SimpleObjectA>(instantA);
            gameObjectB = server.NewGameObject<SimpleObjectB>(instantB);

            idA = server.GetGameObjectID(gameObjectA);
            idB = server.GetGameObjectID(gameObjectB);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Logger.JoinWriter();
        }

        [TestMethod]
        public void GameObjectCollectionSerializeDeserializeTest()
        {
            byte[] serializationA = server.Serialize(gameObjectA, instantA);
            byte[] serializationB = server.Serialize(gameObjectB, instantB);
            client.Deserialize(serializationA);
            client.Deserialize(serializationB);
            SimpleObjectA actualA = (SimpleObjectA)client.GetGameObject(idA);
            actualA.IsIdentical(instantA, gameObjectA, instantA);
            SimpleObjectB actualB = (SimpleObjectB)client.GetGameObject(idB);
            actualA.IsIdentical(instantB, gameObjectB, instantB);

            Assert.IsTrue(server.CheckObjectIdDictionary());
            Assert.IsTrue(client.CheckObjectIdDictionary());
        }
    }
}
