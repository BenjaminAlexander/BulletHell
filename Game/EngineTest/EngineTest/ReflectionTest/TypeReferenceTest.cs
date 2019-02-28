using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EngineTest.EngineTest.TestUtils;
using MyGame.Engine.Reflection;
using MyGame.Engine.GameState;
using Microsoft.Xna.Framework;

namespace EngineTest.EngineTest.ReflectionTest
{
    [TestClass]
    public class TypeReferenceTest
    {
        [TestMethod]
        public void GetTypeTest()
        {
            TypeRefernce<GameObject> typeReference = new TypeRefernce<GameObject>();
            SimpleObjectB objB = SimpleObjectB.Factory(0, 0, 0, 0, 0);
            SimpleObjectA objA = SimpleObjectA.Factory(0, 0, new Vector2(0), 0);

            int typeIDB = typeReference.GetTypeID(objB);
            Type actualB = typeReference.GetType(typeIDB);

            int typeIDA = typeReference.GetTypeID(objA);
            Type actualA = typeReference.GetType(typeIDA);

            Assert.AreEqual(objB.GetType(), actualB);
            Assert.AreEqual(objA.GetType(), actualA);
            Assert.AreNotEqual(actualA, actualB);
            Assert.AreNotEqual(typeIDB, typeIDA);
        }

        [TestMethod]
        public void ConstructTest()
        {
            TypeRefernce<GameObject> typeReference = new TypeRefernce<GameObject>();
            SimpleObjectA objA = SimpleObjectA.Factory(0, 0, new Vector2(0), 0);
            SimpleObjectB objB = SimpleObjectB.Factory(0, 0, 0, 0, 0);

            int typeIDA = typeReference.GetTypeID(objA);
            int typeIDB = typeReference.GetTypeID(objB);

            GameObject reconstructA = typeReference.Construct(typeIDA, new object[0]);
            GameObject reconstructB = typeReference.Construct(typeIDB, new object[0]);

            Assert.AreEqual(objA.GetType(), reconstructA.GetType());
            Assert.AreEqual(objB.GetType(), reconstructB.GetType());
            Assert.AreNotEqual(reconstructA.GetType(), reconstructB.GetType());
        }
    }
}
