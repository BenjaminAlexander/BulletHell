using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.Serialization;
using MyGame.Engine.GameState;
using EngineTest.EngineTest.TestUtils;
using Microsoft.Xna.Framework;
using MyGame.Engine.Reflection;

namespace EngineTest.EngineTest.SerializationTest
{
    [TestClass]
    public class TypeSerializerTest
    {
        [TestMethod]
        public void SerializeDeserializeTest()
        {
            NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddItem<SimpleObjectA>();
            factory.AddItem<SimpleObjectB>();

            TypeSerializer<GameObject> serializer = new TypeSerializer<GameObject>(factory);

            SimpleObjectA expected = SimpleObjectA.Factory(0, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expected.CurrentInstant = 0;

            byte[] serialization = new byte[serializer.SerializationSize(expected)];
            serializer.Serialize(expected, serialization, 0);

            int bufferOffset = 0;
            GameObject actual = serializer.Deserialize(serialization, ref bufferOffset);
            SimpleObjectA actualA = (SimpleObjectA)actual;

            Assert.AreEqual(expected.IntegerMember(0), actualA.IntegerMember(0));
            Assert.AreEqual(expected.FloatMember(0), actualA.FloatMember(0));
            Assert.AreEqual(expected.Vector2Member(0), actualA.Vector2Member(0));
        }

        [TestMethod]
        public void SerializeDeserializeExistingObjectTest1()
        {
            NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddItem<SimpleObjectA>();
            factory.AddItem<SimpleObjectB>();

            TypeSerializer<GameObject> serializer = new TypeSerializer<GameObject>(factory);

            SimpleObjectA expected = SimpleObjectA.Factory(0, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expected.CurrentInstant = 0;

            byte[] serialization = new byte[serializer.SerializationSize(expected)];
            serializer.Serialize(expected, serialization, 0);

            SimpleObjectA actualA = new SimpleObjectA();

            int bufferOffset = 0;
            serializer.Deserialize(actualA, serialization, ref bufferOffset);

            Assert.AreEqual(expected.IntegerMember(0), actualA.IntegerMember(0));
            Assert.AreEqual(expected.FloatMember(0), actualA.FloatMember(0));
            Assert.AreEqual(expected.Vector2Member(0), actualA.Vector2Member(0));
        }
    }
}
