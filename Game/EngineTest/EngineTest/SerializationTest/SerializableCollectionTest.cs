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
            NewConstraintTypeFactory<SerializableDeserializable> factory = new NewConstraintTypeFactory<SerializableDeserializable>();
            factory.AddItem<SerializableInteger>();
            factory.AddItem<SerializableVector2>();

            SerializableCollection<SerializableDeserializable> expectedCollection = new SerializableCollection<SerializableDeserializable>(factory);
            SerializableInteger expectedB = new SerializableInteger(23);
            SerializableVector2 expectedA = new SerializableVector2(new Vector2 (34, 11));

            int expectedIdB = expectedCollection.Add(expectedB);
            int expectedIdA = expectedCollection.Add(expectedA);

            byte[] serializationA = expectedCollection.SerializeObject(expectedIdA);
            byte[] serializationB = expectedCollection.SerializeObject(expectedIdB);

            SerializableCollection<SerializableDeserializable> actualCollection = new SerializableCollection<SerializableDeserializable>(factory);
            int actualIdA = actualCollection.DeserializeObject(serializationA);
            int actualIdB = actualCollection.DeserializeObject(serializationB);

            SerializableInteger actualB = (SerializableInteger)actualCollection.GetObject(actualIdB);
            SerializableVector2 actualA = (SerializableVector2)actualCollection.GetObject(actualIdA);

            Assert.AreEqual(expectedIdA, actualIdA);
            Assert.AreEqual(expectedIdB, actualIdB);
            Assert.AreEqual(expectedA.Value, actualA.Value);

            expectedA.Value = new Vector2(56, 78);
            serializationA = expectedCollection.SerializeObject(expectedIdA);
            actualCollection.DeserializeObject(serializationA);

            Assert.AreEqual(expectedA.Value, actualA.Value);
        }
    }
}
