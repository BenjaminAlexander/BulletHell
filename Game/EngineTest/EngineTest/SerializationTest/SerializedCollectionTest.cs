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
            factory.AddItem<SInteger>();
            factory.AddItem<MyGame.Engine.Serialization.DataTypes.SVector2>();

            SerializedCollection<Serializable> expectedCollection = new SerializedCollection<Serializable>(factory, new SerializableSerializer<Serializable>());
            SInteger expectedB = new SInteger(23);
            SVector2 expectedA = new SVector2(new Vector2 (34, 11));

            int expectedIdB = expectedCollection.Add(expectedB);
            int expectedIdA = expectedCollection.Add(expectedA);

            byte[] serializationA = expectedCollection.SerializeObject(expectedIdA);
            byte[] serializationB = expectedCollection.SerializeObject(expectedIdB);

            SerializedCollection<Serializable> actualCollection = new SerializedCollection<Serializable>(factory, new SerializableSerializer<Serializable>());

            SInteger actualB = (SInteger)actualCollection.Deserialize(serializationB);
            MyGame.Engine.Serialization.DataTypes.SVector2 actualA = (MyGame.Engine.Serialization.DataTypes.SVector2)actualCollection.Deserialize(serializationA);

            int actualIdA = actualCollection.GetID(actualA);
            int actualIdB = actualCollection.GetID(actualB);

            Assert.AreEqual(expectedIdA, actualIdA);
            Assert.AreEqual(expectedIdB, actualIdB);
            Assert.AreEqual(expectedA.Value, actualA.Value);

            expectedA.Value = new Microsoft.Xna.Framework.Vector2(56, 78);
            serializationA = expectedCollection.SerializeObject(expectedIdA);
            actualCollection.Deserialize(serializationA);

            Assert.AreEqual(expectedA.Value, actualA.Value);
        }
    }
}
