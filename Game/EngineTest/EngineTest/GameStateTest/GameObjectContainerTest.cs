using EngineTest.EngineTest.TestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using MyGame.Engine.GameState;
using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineTest.EngineTest.GameStateTest
{
    [TestClass]
    public class GameObjectContainerTest
    {
        GameObjectContainer containerA;

        [TestInitialize]
        public void TestInitialize()
        {
            GameObjectContainer.AddType<SimpleObjectA>();
            GameObjectContainer.AddType<SimpleObjectB>();

            containerA = SimpleObjectA.Factory(0, 1234, new Vector2(656.34f, 345.4f), 787.9f);
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            byte[] serialization = containerA.Serialize();

            GameObjectContainer actual = new GameObjectContainer(serialization);

            Assert.IsTrue(GameObjectContainer.IsIdentical(containerA, actual));
        }

        [TestMethod]
        public void UpdateTest()
        {
            GameObjectContainer container = SimpleObjectA.Factory(0, 0, new Vector2(0), 0);
            GameObjectContainer nextContainer = new GameObjectContainer(container);
            SimpleObjectA actual = (SimpleObjectA)nextContainer.GameObject;
            Assert.AreEqual(new Vector2(1), actual.Vector2Member(nextContainer.Current));
            Assert.AreEqual(1, nextContainer.Instant);
        }
    }
}
