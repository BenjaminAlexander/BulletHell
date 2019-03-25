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
            NewConstraintTypeFactory<InstantSerializable> factory = new NewConstraintTypeFactory<InstantSerializable>();
            factory.AddType<SimpleObjectA>();
            factory.AddType<SimpleObjectB>();

            InstantSerializedCollection<InstantSerializable> expectedCollection = new InstantSerializedCollection<InstantSerializable>(factory);

            expectedCollection.Add(expectedB);
            expectedCollection.Add(expectedA);

            byte[] serializationA = expectedCollection.Serialize(expectedA, 1);
            byte[] serializationB = expectedCollection.Serialize(expectedB, 1);

            InstantSerializedCollection<InstantSerializable> actualCollection = new InstantSerializedCollection<InstantSerializable>(factory);

            SimpleObjectB actualB = (SimpleObjectB)actualCollection.Deserialize(serializationB);
            SimpleObjectA actualA = (SimpleObjectA)actualCollection.Deserialize(serializationA);
            actualB.SetDependencies(instantController);
            actualA.SetDependencies(instantController);

            SimpleObjectA.AssertValuesEqual(expectedA, actualA);

            expectedA.Vector2Member(new Vector2(56, 78));
            serializationA = expectedCollection.Serialize(expectedA, 0);
            actualCollection.Deserialize(serializationA);

            Assert.AreEqual(expectedA.Vector2Member(), actualA.Vector2Member());
        }
    }
}
