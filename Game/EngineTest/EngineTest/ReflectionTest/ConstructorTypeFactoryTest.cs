﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EngineTest.EngineTest.TestUtils;
using MyGame.Engine.Reflection;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;

namespace EngineTest.EngineTest.ReflectionTest
{
    [TestClass]
    public class ConstructorTypeFactoryTest
    {
        SimpleObjectA expectedA;
        SimpleObjectB expectedB;

        [TestInitialize]
        public void TestInitialize()
        {
            expectedA = new SimpleObjectA();
            expectedB = new SimpleObjectB();
        }

        [TestMethod]
        public void GetTypeTest()
        {
            ConstructorTypeFactory<GameObject> typeReference = new ConstructorTypeFactory<GameObject>();
            SimpleObjectB objB = new SimpleObjectB();
            SimpleObjectA objA = new SimpleObjectA();

            int typeIDB = typeReference.GetTypeID(objB);
            Type actualB = typeReference.GetTypeFromID(typeIDB);

            int typeIDA = typeReference.GetTypeID(objA);
            Type actualA = typeReference.GetTypeFromID(typeIDA);

            Assert.AreEqual(objB.GetType(), actualB);
            Assert.AreEqual(objA.GetType(), actualA);
            Assert.AreNotEqual(actualA, actualB);
            Assert.AreNotEqual(typeIDB, typeIDA);
        }

        [TestMethod]
        public void ConstructTest()
        {
            ConstructorTypeFactory<GameObject> typeReference = new ConstructorTypeFactory<GameObject>();

            int typeIDA = typeReference.GetTypeID(expectedA);
            int typeIDB = typeReference.GetTypeID(expectedB);

            GameObject reconstructA = typeReference.Construct(typeIDA, new object[0]);
            GameObject reconstructB = typeReference.Construct(typeIDB, new object[0]);

            Assert.AreEqual(expectedA.GetType(), reconstructA.GetType());
            Assert.AreEqual(expectedB.GetType(), reconstructB.GetType());
            Assert.AreNotEqual(reconstructA.GetType(), reconstructB.GetType());
        }
    }
}
