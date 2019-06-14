/*using System;
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
        NewConstraintTypeFactory<GameObject> factory;
        SimpleObjectA expectedA;
        SimpleObjectB expectedB;

        [TestInitialize]
        public void TestInitialize()
        {
            factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddType<SimpleObjectA>();
            factory.AddType<SimpleObjectB>();
            expectedA = new SimpleObjectA();
            expectedB = new SimpleObjectB();
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            TypeSerializer<GameObject> serializer = new TypeSerializer<GameObject>(factory);

            byte[] serialization = serializer.Serialize(expectedA);

            GameObject actual = serializer.Deserialize(serialization);
            SimpleObjectA actualA = (SimpleObjectA)actual;
            SimpleObjectA.AssertValuesEqual(expectedA, actualA);
        }

        [TestMethod]
        public void SerializeDeserializeExistingObjectTest1()
        {
            TypeSerializer<GameObject> serializer = new TypeSerializer<GameObject>(factory);

            byte[] serialization = serializer.Serialize(expectedA);

            SimpleObjectA actualA = GameObject.NewObject<SimpleObjectA>(0);

            serializer.Deserialize(actualA, serialization);
            SimpleObjectA.AssertValuesEqual(expectedA, actualA);
        }
    }
}*/
