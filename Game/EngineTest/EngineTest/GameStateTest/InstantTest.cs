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
    public class InstantTest
    {
        Instant instant;

        [TestInitialize]
        public void TestInitialize()
        {
            instant = new Instant(123);
        }

        [TestMethod]
        public void InstantSerializeDeserializeTest()
        {
            int offset = 0;
            byte[] serialization = new byte[instant.SerializationSize];
            instant.Serialize(serialization, ref offset);
            offset = 0;

            Instant newInstant = new Instant(serialization, ref offset);

            Assert.AreEqual(instant, newInstant);
        }
    }
}
