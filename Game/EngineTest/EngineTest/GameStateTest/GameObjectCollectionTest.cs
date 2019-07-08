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
        Instant serverInstantA;
        Instant serverInstantB;

        [TestInitialize]
        public void TestInitialize()
        {
            Logger.StartLogger();

            GameObject.AddType<SimpleObjectA>();
            GameObject.AddType<SimpleObjectB>();

            server = new GameObjectCollection();
            client = new GameObjectCollection();

            serverInstantA = server.GetInstant(123);
            serverInstantB = server.GetInstant(65);

            gameObjectA = SimpleObjectA.Factory(serverInstantA, 0, new Vector2(0), 0);
            gameObjectB = SimpleObjectB.Factory(serverInstantB, 0,0,0,0);

            idA = (int)gameObjectA.ID;
            idB = (int)gameObjectB.ID;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Logger.JoinWriter();
        }

        [TestMethod]
        public void GameObjectCollectionSerializeDeserializeTest()
        {
            byte[] serializationA = server.Serialize(gameObjectA, serverInstantA);
            byte[] serializationB = server.Serialize(gameObjectB, serverInstantB);
            client.Deserialize(serializationA);
            client.Deserialize(serializationB);
            SimpleObjectA actualA = (SimpleObjectA)client.GetGameObject(idA);
            Instant clientInstantA = client.GetInstant(123);
            actualA.IsIdentical(clientInstantA, gameObjectA, serverInstantA);
            SimpleObjectB actualB = (SimpleObjectB)client.GetGameObject(idB);
            Instant clientInstantB = client.GetInstant(65);
            actualA.IsIdentical(clientInstantB, gameObjectB, serverInstantB);

            Assert.IsTrue(server.CheckCollectionIntegrety());
            Assert.IsTrue(client.CheckCollectionIntegrety());

            server.Update(123);
            Assert.IsTrue(server.CheckCollectionIntegrety());
        }
    }
}
