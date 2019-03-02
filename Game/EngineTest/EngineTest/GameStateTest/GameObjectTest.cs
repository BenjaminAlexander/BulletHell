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
            SimpleObjectA expected = SimpleObjectA.Factory(new Instant(0), 1234, new Vector2(656.34f, 345.4f), 787.9f);
            byte[] serialization = new byte[expected.SerializationSize];
            expected.Serialize(null, serialization, 0);

            SimpleObjectA actual = new SimpleObjectA();
            actual.Deserialize(null, serialization, 0);

            Assert.AreEqual(expected.IntegerMember(null), actual.IntegerMember(null));
            Assert.AreEqual(expected.FloatMember(null), actual.FloatMember(null));
            Assert.AreEqual(expected.Vector2Member(null), actual.Vector2Member(null));
        }
    }
}
