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
    public class SerializableCollectionTest
    {
        [TestMethod]
        public void SerializeDeserializeTest()
        {
            NewConstraintTypeFactory<Serializable> factory = new NewConstraintTypeFactory<Serializable>();
            factory.AddItem<SerializableInteger>();
            factory.AddItem<SerializableVector2>();

            SerializableCollection<Serializable> expectedCollection = new SerializableCollection<Serializable>(factory);
            SerializableInteger expectedB = new SerializableInteger(23);
            SerializableVector2 expectedA = new SerializableVector2(new Vector2 (34, 11));

            int expectedIdB = expectedCollection.Add(expectedB);
            int expectedIdA = expectedCollection.Add(expectedA);

            byte[] serializationA = expectedCollection.SerializeObject(expectedIdA);
            byte[] serializationB = expectedCollection.SerializeObject(expectedIdB);

            SerializableCollection<Serializable> actualCollection = new SerializableCollection<Serializable>(factory);

            SerializableInteger actualB = (SerializableInteger)actualCollection.Deserialize(serializationB);
            SerializableVector2 actualA = (SerializableVector2)actualCollection.Deserialize(serializationA);

            int actualIdA = actualCollection.GetID(actualA);
            int actualIdB = actualCollection.GetID(actualB);

            Assert.AreEqual(expectedIdA, actualIdA);
            Assert.AreEqual(expectedIdB, actualIdB);
            Assert.AreEqual(expectedA.Value, actualA.Value);

            expectedA.Value = new Vector2(56, 78);
            serializationA = expectedCollection.SerializeObject(expectedIdA);
            actualCollection.Deserialize(serializationA);

            Assert.AreEqual(expectedA.Value, actualA.Value);
        }
    }
}
