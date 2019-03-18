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
    public class TypeSerializerTest
    {
        [TestMethod]
        public void SerializeDeserializeTest()
        {
            NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddItem<SimpleObjectA>();
            factory.AddItem<SimpleObjectB>();
            GameObjectTestUtils utils = new GameObjectTestUtils();

            TypeSerializer<GameObject> serializer = new TypeSerializer<GameObject>(factory, utils.instantController, utils.instantController);

            //byte[] serialization = Utils.Serialize<GameObject>(expected.InstantSelector, expected);
            byte[] serialization = new byte[serializer.SerializationSize(utils.expectedA)];
            int offset = 0;
            serializer.Serialize(utils.expectedA, serialization, ref offset);

            int bufferOffset = 0;
            GameObject actual = serializer.Deserialize(serialization, ref bufferOffset);
            SimpleObjectA actualA = (SimpleObjectA)actual;

            SimpleObjectA.AssertValuesEqual(utils.expectedA, actualA);
        }

        [TestMethod]
        public void SerializeDeserializeExistingObjectTest1()
        {
            NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddItem<SimpleObjectA>();
            factory.AddItem<SimpleObjectB>();

            GameObjectTestUtils utils = new GameObjectTestUtils();

            TypeSerializer<GameObject> serializer = new TypeSerializer<GameObject>(factory, utils.instantController, utils.instantController);

            //byte[] serialization = Utils.Serialize<GameObject>(expected.InstantSelector, expected);
            byte[] serialization = new byte[serializer.SerializationSize(utils.expectedA)];
            int offset = 0;
            serializer.Serialize(utils.expectedA, serialization, ref offset);

            SimpleObjectA actualA = GameObject.Construct<SimpleObjectA>(utils.instantController);


            int bufferOffset = 0;
            serializer.Deserialize(actualA, serialization, ref bufferOffset);

            SimpleObjectA.AssertValuesEqual(utils.expectedA, actualA);
        }
    }
}
