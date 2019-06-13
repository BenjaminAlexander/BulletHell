using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EngineTest.EngineTest.TestUtils;
using MyGame.Engine.Reflection;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;

namespace EngineTest.EngineTest.ReflectionTest
{
    [TestClass]
    public class NewConstraintTypeFactoryTest
    {
        NewConstraintTypeFactory<GameObject> factory;
        SimpleObjectA expectedA;
        SimpleObjectB expectedB;
        int instant = 0;

        [TestInitialize]
        public void TestInitialize()
        {
            factory = new NewConstraintTypeFactory<GameObject>();
            factory.AddType<SimpleObjectA>();
            factory.AddType<SimpleObjectB>();
            expectedA = SimpleObjectA.Factory<SimpleObjectA>(instant, 1234, new Vector2(656.34f, 345.4f), 787.9f);
            expectedB = new SimpleObjectB();
        }

        [TestMethod]
        public void GetTypeTest()
        {
            int typeIDB = factory.GetTypeID(expectedB);
            Type actualB = factory.GetTypeFromID(typeIDB);

            int typeIDA = factory.GetTypeID(expectedA);
            Type actualA = factory.GetTypeFromID(typeIDA);

            Assert.AreEqual(expectedB.GetType(), actualB);
            Assert.AreEqual(expectedA.GetType(), actualA);
            Assert.AreNotEqual(actualA, actualB);
            Assert.AreNotEqual(typeIDB, typeIDA);
        }

        [TestMethod]
        public void ConstructTest()
        {
            int typeIDA = factory.GetTypeID(expectedA);
            int typeIDB = factory.GetTypeID(expectedB);

            GameObject reconstructA = factory.Construct(typeIDA);
            GameObject reconstructB = factory.Construct(typeIDB);

            Assert.AreEqual(expectedA.GetType(), reconstructA.GetType());
            Assert.AreEqual(expectedB.GetType(), reconstructB.GetType());
            Assert.AreNotEqual(reconstructA.GetType(), reconstructB.GetType());
        }
    }
}
