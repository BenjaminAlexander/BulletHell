using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;
using EngineTest.EngineTest.TestUtils;

namespace EngineTest.EngineTest.GameStateTest
{
    [TestClass]
    public class GameObjectTest
    {
        [TestMethod]
        public void SerializeDeserializeTest()
        {
            SimpleObjectA expected = SimpleObjectA.Factory(0, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            byte[] serialization = new byte[expected.SerializationSize];
            expected.Serialize(0, serialization, 0);

            SimpleObjectA actual = new SimpleObjectA();
            actual.Deserialize(serialization, 0);

            Assert.AreEqual(expected.IntegerMember(0), actual.IntegerMember(0));
            Assert.AreEqual(expected.FloatMember(0), actual.FloatMember(0));
            Assert.AreEqual(expected.Vector2Member(0), actual.Vector2Member(0));
        }
    }
}
