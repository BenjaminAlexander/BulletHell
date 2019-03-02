using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EngineTest.EngineTest.TestUtils;
using MyGame.Engine.Reflection;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;

namespace EngineTest.EngineTest.ReflectionTest
{
    [TestClass]
    public class DerivedTypeFactoryTest
    {
        [TestMethod]
        public void GetTypeTest()
        {
            DerivedTypeFactory<GameObject> factory = new DerivedTypeFactory<GameObject>();
            factory.AddItem<SimpleObjectA>();
            factory.AddItem<SimpleObjectB>();
            SimpleObjectB objB = SimpleObjectB.Factory(new Instant(0), 0, 0, 0, 0);
            SimpleObjectA objA = SimpleObjectA.Factory(new Instant(0), 0, new Vector2(0), 0);

            int typeIDB = factory.GetTypeID(objB);
            Type actualB = factory.GetTypeFromID(typeIDB);

            int typeIDA = factory.GetTypeID(objA);
            Type actualA = factory.GetTypeFromID(typeIDA);

            Assert.AreEqual(objB.GetType(), actualB);
            Assert.AreEqual(objA.GetType(), actualA);
            Assert.AreNotEqual(actualA, actualB);
            Assert.AreNotEqual(typeIDB, typeIDA);
        }

        [TestMethod]
        public void ConstructTest()
        {
            DerivedTypeFactory<GameObject> factory = new DerivedTypeFactory<GameObject>();
            factory.AddItem<SimpleObjectA>();
            factory.AddItem<SimpleObjectB>();
            SimpleObjectA objA = SimpleObjectA.Factory(new Instant(0), 0, new Vector2(0), 0);
            SimpleObjectB objB = SimpleObjectB.Factory(new Instant(0), 0, 0, 0, 0);

            int typeIDA = factory.GetTypeID(objA);
            int typeIDB = factory.GetTypeID(objB);

            GameObject reconstructA = factory.Construct(typeIDA);
            GameObject reconstructB = factory.Construct(typeIDB);

            Assert.AreEqual(objA.GetType(), reconstructA.GetType());
            Assert.AreEqual(objB.GetType(), reconstructB.GetType());
            Assert.AreNotEqual(reconstructA.GetType(), reconstructB.GetType());
        }
    }
}
