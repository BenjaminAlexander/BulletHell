using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.Serialization;
using MyGame.Engine.GameState;
using EngineTest.EngineTest.TestUtils;
using Microsoft.Xna.Framework;

namespace EngineTest.EngineTest.SerializationTest
{
    [TestClass]
    public class DerivedTypeSerializerTest
    {
        [TestMethod]
        public void SerializeDeserializeTest()
        {
            DerivedTypeSerializer<GameObject> serializer = new DerivedTypeSerializer<GameObject>();
            serializer.AddItem<SimpleObjectA>();
            serializer.AddItem<SimpleObjectB>();

            SimpleObjectA expected = SimpleObjectA.Factory(0, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expected.CurrentInstant = 0;

            byte[] serialization = new byte[serializer.SerializationSize(expected)];
            serializer.Serialize(expected, serialization, 0);

            int instant = 0;
            GameObject actual = serializer.Deserialize(serialization, ref instant);
            SimpleObjectA actualA = (SimpleObjectA)actual;

            Assert.AreEqual(expected.IntegerMember(0), actualA.IntegerMember(0));
            Assert.AreEqual(expected.FloatMember(0), actualA.FloatMember(0));
            Assert.AreEqual(expected.Vector2Member(0), actualA.Vector2Member(0));
        }
    }
}
