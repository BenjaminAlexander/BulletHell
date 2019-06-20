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
        SimpleObjectA gameObjectA;
        Instant instant;

        [TestInitialize]
        public void TestInitialize()
        {
            GameObject.AddType<SimpleObjectA>();
            GameObject.AddType<SimpleObjectB>();

            instant = new Instant(123);
            gameObjectA = SimpleObjectA.Factory(instant, 1234, new Vector2(656.34f, 345.4f), 787.9f);
        }

        [TestMethod]
        public void GameObjectSerializeDeserializeTest()
        {
            byte[] serialization = gameObjectA.Serialize(instant);

            GameObject actualGameObject = GameObject.Construct(serialization, 0);

            int bufferOffset = 0;
            actualGameObject.Deserialize(serialization, ref bufferOffset);

            Assert.IsTrue(actualGameObject.IsIdentical(instant, gameObjectA, instant));
        }

        [TestMethod]
        public void UpdateTest()
        {
            instant = new Instant(0);
            gameObjectA = SimpleObjectA.Factory(instant, 0, new Vector2(0), 0);

            Instant nextContainer = instant.GetNext;
            gameObjectA.CallUpdate(instant.AsCurrent, nextContainer.AsNext);


            Assert.AreEqual(new Vector2(1), gameObjectA.Vector2Member(nextContainer.AsCurrent));
            Assert.AreEqual(new Instant(1), nextContainer);
        }
    }
}
