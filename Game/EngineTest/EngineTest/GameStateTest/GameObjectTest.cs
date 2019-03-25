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
            expectedB.SetDependencies(instantController);
            instantController.AdvanceReadWriteInstant();
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            byte[] serialization = Utils.Serialize(expectedA, 1);

            SimpleObjectA actual = GameObject.NewObject<SimpleObjectA>(instantController);
            Utils.Deserialize(actual, serialization);

            SimpleObjectA.AssertValuesEqual(expectedA, actual);
            actual.CopyInstant(1, 2);
            Assert.IsTrue(actual.StateAtInstantExists(2));

            serialization = Utils.Serialize(expectedA, 2);
            Utils.Deserialize(actual, serialization);
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
