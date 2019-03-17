﻿using System;
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

            TypeSerializer<GameObject> serializer = new TypeSerializer<GameObject>(factory);

            SimpleObjectA expected = SimpleObjectA.Factory(1234, new Vector2(656.34f, 345.4f), 787.9f);

            byte[] serialization = new byte[serializer.SerializationSize(new SerializableSerializer<GameObject>(), expected)];
            serializer.Serialize(new SerializableSerializer<GameObject>(), expected, serialization, 0);

            int bufferOffset = 0;
            GameObject actual = serializer.Deserialize(new DeserializableDeserializer<GameObject>(), serialization, ref bufferOffset);
            SimpleObjectA actualA = (SimpleObjectA)actual;

            SimpleObjectA.AssertValuesEqual(expected, actualA);
        }

        [TestMethod]
        public void SerializeDeserializeExistingObjectTest1()
        {
            InstantSelector.InstantController instant = new InstantSelector.InstantController();

            NewConstraintTypeFactory<GameObject> factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddItem<SimpleObjectA>();
            factory.AddItem<SimpleObjectB>();

            TypeSerializer<GameObject> serializer = new TypeSerializer<GameObject>(factory);

            SimpleObjectA expected = SimpleObjectA.Factory(1234, new Vector2(656.34f, 345.4f), 787.9f);
            expected.InstantSelector = instant;

            byte[] serialization = new byte[serializer.SerializationSize(new SerializableSerializer<GameObject>(), expected)];
            serializer.Serialize(new SerializableSerializer<GameObject>(), expected, serialization, 0);

            SimpleObjectA actualA = new SimpleObjectA();

            int bufferOffset = 0;
            serializer.Deserialize(new DeserializableDeserializer<GameObject>(), actualA, serialization, ref bufferOffset);

            SimpleObjectA.AssertValuesEqual(expected, actualA);
        }
    }
}
