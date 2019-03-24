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
            NewConstraintTypeFactory<InstantSerializable> factory = new NewConstraintTypeFactory<InstantSerializable>();
            factory.AddType<SInteger>();
            factory.AddType<SVector2>();

            SerializedCollection<InstantSerializable> expectedCollection = new SerializedCollection<InstantSerializable>(factory);
            SInteger expectedB = new SInteger(23);
            SVector2 expectedA = new SVector2(new Vector2 (34, 11));

            int expectedIdB = expectedCollection.AddItem(expectedB);
            int expectedIdA = expectedCollection.AddItem(expectedA);

            byte[] serializationA = expectedCollection.SerializeObject(expectedIdA, 0);
            byte[] serializationB = expectedCollection.SerializeObject(expectedIdB, 0);

            SerializedCollection<InstantSerializable> actualCollection = new SerializedCollection<InstantSerializable>(factory);

            SInteger actualB = (SInteger)actualCollection.Deserialize(serializationB);
            MyGame.Engine.Serialization.DataTypes.SVector2 actualA = (MyGame.Engine.Serialization.DataTypes.SVector2)actualCollection.Deserialize(serializationA);

            int actualIdA = actualCollection.GetID(actualA);
            int actualIdB = actualCollection.GetID(actualB);

            Assert.AreEqual(expectedIdA, actualIdA);
            Assert.AreEqual(expectedIdB, actualIdB);
            Assert.AreEqual(expectedA.Value, actualA.Value);

            expectedA.Value = new Microsoft.Xna.Framework.Vector2(56, 78);
            serializationA = expectedCollection.SerializeObject(expectedIdA, 0);
            actualCollection.Deserialize(serializationA);

            Assert.AreEqual(expectedA.Value, actualA.Value);
        }
    }
}
