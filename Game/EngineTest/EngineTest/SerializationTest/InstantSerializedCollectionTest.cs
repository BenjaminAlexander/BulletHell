using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;
using MyGame.Engine.GameState;
using EngineTest.EngineTest.TestUtils;
using Microsoft.Xna.Framework;
using MyGame.Engine.Reflection;

namespace EngineTest.EngineTest.SerializationTest
{
    [TestClass]
    public class InstantSerializedCollectionTest
    {
        SimpleObjectA expectedA;
        SimpleObjectB expectedB;

        [TestInitialize]
        public void TestInitialize()
        {
            expectedA = SimpleObjectA.Factory<SimpleObjectA>(0, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expectedB = new SimpleObjectB();
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            NewConstraintTypeFactory<Serializable> factory = new NewConstraintTypeFactory<Serializable>();
            factory.AddType<SimpleObjectA>();
            factory.AddType<SimpleObjectB>();

            SerializedCollection<Serializable> expectedCollection = new SerializedCollection<Serializable>(factory);

            expectedCollection.Add(expectedB);
            expectedCollection.Add(expectedA);

            byte[] serializationA = expectedCollection.Serialize(expectedA);
            byte[] serializationB = expectedCollection.Serialize(expectedB);

            SerializedCollection<Serializable> actualCollection = new SerializedCollection<Serializable>(factory);

            SimpleObjectB actualB = (SimpleObjectB)actualCollection.Deserialize(serializationB);
            SimpleObjectA actualA = (SimpleObjectA)actualCollection.Deserialize(serializationA);

            SimpleObjectA.AssertValuesEqual(expectedA, actualA);

            expectedA.Vector2Member(new Vector2(56, 78));
            serializationA = expectedCollection.Serialize(expectedA);
            actualCollection.Deserialize(serializationA);

            Assert.AreEqual(expectedA.Vector2Member(), actualA.Vector2Member());
        }
    }
}
