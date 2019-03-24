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
        SimpleInstantSelector instantController;

        [TestInitialize]
        public void TestInitialize()
        {
            instantController = new SimpleInstantSelector();
            expectedA = SimpleObjectA.Factory<SimpleObjectA>(instantController, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expectedB = new SimpleObjectB();
            expectedB.InstantSelector = instantController;
            instantController.AdvanceReadWriteInstant();
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            byte[] serialization = expectedA.Serialize(1);

            SimpleObjectA actual = GameObject.Factory<SimpleObjectA>(instantController);
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
            instantController.SetReadWriteInstant(10);
            SimpleObjectA expected = SimpleObjectA.Factory<SimpleObjectA>(instantController, 0, new Vector2(0), 0);

            instantController.AdvanceReadWriteInstant();
            expected.Update(instantController.ReadInstant, instantController.WriteInstant);

            instantController.AdvanceReadWriteInstant();
            Assert.AreEqual(new Vector2(1), expected.Vector2Member());
        }
    }
}
