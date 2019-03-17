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
    public class SerializableCollectionTest
    {
        [TestMethod]
        public void SerializeDeserializeTest()
        {
            NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddItem<SimpleObjectA>();
            factory.AddItem<SimpleObjectB>();

            FullTypeSerializer<GameObject> serializer = new FullTypeSerializer<GameObject>(factory);

            FullSerializableCollection<GameObject> expectedCollection = new FullSerializableCollection<GameObject>(serializer);
            SimpleObjectB expectedB = SimpleObjectB.Factory(0, 0, 0, 0);
            SimpleObjectA expectedA = SimpleObjectA.Factory(0, new Vector2(0), 0);

            int expectedIdB = expectedCollection.Add(expectedB);
            int expectedIdA = expectedCollection.Add(expectedA);

            byte[] serializationA = expectedCollection.SerializeObject(expectedIdA);
            byte[] serializationB = expectedCollection.SerializeObject(expectedIdB);

            FullSerializableCollection<GameObject> actualCollection = new FullSerializableCollection<GameObject>(serializer);
            int actualIdA = actualCollection.DeserializeObject(serializationA);
            int actualIdB = actualCollection.DeserializeObject(serializationB);

            SimpleObjectB actualB = (SimpleObjectB)actualCollection.GetObject(actualIdB);
            SimpleObjectA actualA = (SimpleObjectA)actualCollection.GetObject(actualIdA);

            Assert.AreEqual(expectedIdA, actualIdA);
            Assert.AreEqual(expectedIdB, actualIdB);
            SimpleObjectA.AssertValuesEqual(expectedA, actualA);

            expectedA.IntegerMember(1234);
            serializationA = expectedCollection.SerializeObject(expectedIdA);
            actualCollection.DeserializeObject(serializationA);

            SimpleObjectA.AssertValuesEqual(expectedA, actualA);
        }
    }
}
