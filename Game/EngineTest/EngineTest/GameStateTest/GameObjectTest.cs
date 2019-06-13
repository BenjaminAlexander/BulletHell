using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using EngineTest.EngineTest.TestUtils;
using MyGame.Engine.Serialization;

namespace EngineTest.EngineTest.GameStateTest
{
    [TestClass]
    public class GameObjectTest
    {
        SimpleObjectA expectedA;
        SimpleObjectB expectedB;
        int instant = 0;

        [TestInitialize]
        public void TestInitialize()
        {
            expectedA = SimpleObjectA.Factory<SimpleObjectA>(instant, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expectedB = new SimpleObjectB();
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            byte[] serialization = Utils.Serialize(expectedA);

            SimpleObjectA actual = new SimpleObjectA();
            Utils.Deserialize(actual, serialization);

            SimpleObjectA.AssertValuesEqual(expectedA, actual);
        }

        [TestMethod]
        public void UpdateTest()
        {
            SimpleObjectA obj = SimpleObjectA.Factory<SimpleObjectA>(instant, 0, new Vector2(0), 0);
            SimpleObjectA actual = (SimpleObjectA)obj.NextInstant();
            Assert.AreEqual(new Vector2(1), actual.Vector2Member());
        }
    }
}
