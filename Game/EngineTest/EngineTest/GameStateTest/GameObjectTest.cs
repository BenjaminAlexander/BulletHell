using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using EngineTest.EngineTest.TestUtils;
using MyGame.Engine.Serialization;
using MyGame.Engine.GameState.Instants;

namespace EngineTest.EngineTest.GameStateTest
{
    [TestClass]
    public class GameObjectTest
    {
        GameObjectSet globalSet;
        SimpleObjectA gameObjectA;
        Instant instant;
        Instant actualInstant;

        [TestInitialize]
        public void TestInitialize()
        {
            globalSet = new GameObjectSet();

            GameObject.AddType<SimpleObjectA>();
            GameObject.AddType<SimpleObjectB>();

            instant = new Instant(123, globalSet);
            actualInstant = new Instant(123, globalSet);
            gameObjectA = SimpleObjectA.Factory(instant, 1234, new Vector2(656.34f, 345.4f), 787.9f);
        }

        [TestMethod]
        public void GameObjectSerializeDeserializeTest()
        {
            byte[] serialization = new byte[gameObjectA.SerializationSize(instant.ID)];
            int writeOffset = 0;
            gameObjectA.Serialize(instant.ID, serialization, ref writeOffset);

            int bufferOffset = 0;
            GameObject actualGameObject = GameObject.NewGameObject(0, actualInstant, serialization, bufferOffset);
            actualGameObject.Deserialize(actualInstant, serialization, ref bufferOffset);

            Assert.IsTrue(actualGameObject.IsIdentical(instant, gameObjectA, actualInstant));
        }

        [TestMethod]
        public void UpdateTest()
        {
            instant = new Instant(0, globalSet);
            gameObjectA = SimpleObjectA.Factory(instant, 0, new Vector2(0), 0);

            Instant nextContainer = new Instant(1, globalSet);
            gameObjectA.CallUpdate(instant.AsCurrent, nextContainer.AsNext);

            Assert.AreEqual(new Vector2(1), gameObjectA.Vector2Member(nextContainer.AsCurrent));
            Assert.AreEqual(new Instant(1, globalSet), nextContainer);
        }
    }
}
