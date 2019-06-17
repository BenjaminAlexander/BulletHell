using EngineTest.EngineTest.TestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using MyGame.Engine.GameState;
using MyGame.Engine.GameState.Instants;
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
        SimpleObjectA gameObjectA;
        Instant containerA;

        [TestInitialize]
        public void TestInitialize()
        {
            GameObject.AddType<SimpleObjectA>();
            GameObject.AddType<SimpleObjectB>();

            containerA = new Instant(123);
            gameObjectA = SimpleObjectA.Factory(containerA, 1234, new Vector2(656.34f, 345.4f), 787.9f);
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            byte[] serialization = gameObjectA.Serialize(containerA);

            GameObject actualGameObject = GameObject.Construct(serialization, 0);

            

            int bufferOffset = 0;
            actualGameObject.Deserialize(serialization, ref bufferOffset);

            Assert.IsTrue(actualGameObject.IsIdentical(containerA, gameObjectA, containerA));
        }

        [TestMethod]
        public void UpdateTest()
        {
            containerA = new Instant(0);
            gameObjectA = SimpleObjectA.Factory(containerA, 0, new Vector2(0), 0);

            //GameObjectContainer container = SimpleObjectA.Factory(0, 0, new Vector2(0), 0);
            Instant nextContainer = containerA.GetNext;
            gameObjectA.CallUpdate(containerA.AsCurrent, nextContainer.AsNext);


            //SimpleObjectA actual = (SimpleObjectA)nextContainer.GameObject;
            Assert.AreEqual(new Vector2(1), gameObjectA.Vector2Member(nextContainer.AsCurrent));
            Assert.AreEqual(new Instant(1), nextContainer);
        }
    }
}
