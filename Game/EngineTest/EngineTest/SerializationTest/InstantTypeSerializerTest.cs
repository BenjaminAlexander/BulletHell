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
    public class InstantTypeSerializerTest
    {
        NewConstraintTypeFactory<GameObject> factory;
        SimpleObjectA expectedA;
        SimpleObjectB expectedB;
        SimpleInstantSelector instantController;

        [TestInitialize]
        public void TestInitialize()
        {
            instantController = new SimpleInstantSelector();
            factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddType<SimpleObjectA>();
            factory.AddType<SimpleObjectB>();
            expectedA = SimpleObjectA.Factory<SimpleObjectA>(instantController, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expectedB = new SimpleObjectB();
            instantController.AdvanceReadWriteInstant();
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            InstantTypeSerializer<GameObject> serializer = new InstantTypeSerializer<GameObject>(factory);

            byte[] serialization = serializer.Serialize(expectedA, 1);

            GameObject actual = serializer.Deserialize(serialization);
            SimpleObjectA actualA = (SimpleObjectA)actual;
            actualA.SetDependencies(instantController);

            SimpleObjectA.AssertValuesEqual(expectedA, actualA);
        }

        [TestMethod]
        public void SerializeDeserializeExistingObjectTest1()
        {
            InstantTypeSerializer<GameObject> serializer = new InstantTypeSerializer<GameObject>(factory);

            byte[] serialization = serializer.Serialize(expectedA, 1);

            SimpleObjectA actualA = GameObject.NewObject<SimpleObjectA>(instantController);

            serializer.Deserialize(actualA, serialization);
            SimpleObjectA.AssertValuesEqual(expectedA, actualA);
        }
    }
}
