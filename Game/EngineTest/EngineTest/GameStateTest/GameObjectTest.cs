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
        [TestMethod]
        public void SerializeDeserializeTest()
        {
            SimpleObjectA expected = SimpleObjectA.Factory(1234, new Vector2(656.34f, 345.4f), 787.9f);
            byte[] serialization = new byte[expected.InstantSelector.SerializationSize(expected)];
            int offset = 0;
            expected.InstantSelector.Serialize(expected, serialization, ref offset);

            SimpleObjectA actual = new SimpleObjectA();
            Utils.Deserialize(actual, serialization);

            SimpleObjectA.AssertValuesEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateTest()
        {
            InstantSelector.InstantController instantController = new InstantSelector.InstantController();
            instantController.SetReadInstant(10);
            instantController.SetWriteInstant(11);
            SimpleObjectA expected = SimpleObjectA.Factory(instantController, 0, new Vector2(0), 0);

            instantController.SetReadInstant(11);
            instantController.SetWriteInstant(12);
            expected.Update();

            instantController.SetReadInstant(12);
            instantController.SetWriteInstant(13);
            Assert.AreEqual(new Vector2(1), expected.Vector2Member());

        }
    }
}
