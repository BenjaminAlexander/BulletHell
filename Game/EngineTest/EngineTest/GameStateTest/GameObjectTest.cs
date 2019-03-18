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
            GameObjectTestUtils utils = new GameObjectTestUtils();
            byte[] serialization = Utils.Serialize<GameObject>(utils.instantController, utils.expectedA);

            SimpleObjectA actual = new SimpleObjectA();
            Utils.Deserialize<GameObject>(utils.instantController, actual, serialization);

            SimpleObjectA.AssertValuesEqual(utils.expectedA, actual);
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
            expected.UpdateNextInstant();

            instantController.SetReadInstant(12);
            instantController.SetWriteInstant(13);
            Assert.AreEqual(new Vector2(1), expected.Vector2Member());

        }
    }
}
