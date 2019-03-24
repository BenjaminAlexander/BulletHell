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

        [TestInitialize]
        public void TestInitialize()
        {
            expectedA = SimpleObjectA.Factory<SimpleObjectA>(1234, new Vector2(656.34f, 345.4f), 787.9f);
            expectedB = new SimpleObjectB();
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            byte[] serialization = expectedA.Serialize(1);

            SimpleObjectA actual = new SimpleObjectA();
            actual.Deserialize(serialization);

            SimpleObjectA.AssertValuesEqual(expectedA, actual);
            actual.CopyInstant(1, 2);
            Assert.IsTrue(actual.StateAtInstantExists(2));

            serialization = expectedA.Serialize(2);
            actual.Deserialize(serialization);
            Assert.IsFalse(actual.StateAtInstantExists(2));
        }

        [TestMethod]
        public void UpdateTest()
        {
            GameObject.ReadInstant = 10;
            GameObject.WriteInstant = 11;
            SimpleObjectA expected = SimpleObjectA.Factory<SimpleObjectA>(0, new Vector2(0), 0);

            GameObject.ReadInstant = 11;
            GameObject.WriteInstant = 12;
            expected.UpdateNextInstant();

            GameObject.ReadInstant = 12;
            GameObject.WriteInstant = 13;
            Assert.AreEqual(new Vector2(1), expected.Vector2Member());
        }
    }
}
