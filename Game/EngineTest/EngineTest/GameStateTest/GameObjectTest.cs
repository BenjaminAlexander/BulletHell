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
            byte[] serialization = new byte[expected.SerializationSize];
            expected.Serialize(serialization, 0);

            SimpleObjectA actual = new SimpleObjectA();
            Utils.Deserialize(actual, serialization);

            SimpleObjectA.AssertValuesEqual(expected, actual);
        }
    }
}
