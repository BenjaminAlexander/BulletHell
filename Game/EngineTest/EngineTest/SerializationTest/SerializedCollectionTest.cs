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
    public class SerializedCollectionTest
    {
        [TestMethod]
        public void SerializeDeserializeTest()
        {
            NewConstraintTypeFactory<Serializable> factory = new NewConstraintTypeFactory<Serializable>();
            factory.AddType<SInteger>();
            factory.AddType<SVector2>();

            SerializedCollection<Serializable> expectedCollection = new SerializedCollection<Serializable>(factory);
            SInteger expectedB = new SInteger(23);
            SVector2 expectedA = new SVector2(new Vector2(34, 11));

            expectedCollection.Add(expectedB);
            expectedCollection.Add(expectedA);

            byte[] serializationA = expectedCollection.Serialize(expectedA);
            byte[] serializationB = expectedCollection.Serialize(expectedB);

            SerializedCollection<Serializable> actualCollection = new SerializedCollection<Serializable>(factory);

            SInteger actualB = (SInteger)actualCollection.Deserialize(serializationB);
            SVector2 actualA = (SVector2)actualCollection.Deserialize(serializationA);

            Assert.AreEqual(expectedA.Value, actualA.Value);

            expectedA.Value = new Microsoft.Xna.Framework.Vector2(56, 78);
            serializationA = expectedCollection.Serialize(expectedA);
            actualCollection.Deserialize(serializationA);

            Assert.AreEqual(expectedA.Value, actualA.Value);
        }
    }
}