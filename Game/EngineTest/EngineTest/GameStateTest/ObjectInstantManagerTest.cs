using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EngineTest.EngineTest.TestUtils;
using System.Collections.Generic;

namespace EngineTest.EngineTest.GameStateTest
{
    [TestClass]
    public class ObjectInstantManagerTest
    {
        SimpleEngine serverEngine;
        SimpleEngine clientEngine;

        [TestInitialize]
        public void TestInitialize()
        {
            serverEngine = new SimpleEngine();
            clientEngine = new SimpleEngine();
            serverEngine.ServerInitializeFirstInstant();
        }

        [TestMethod]
        public void TestMethod1()
        {
            List<byte[]> buffers = serverEngine.objectInstantManager.Serialize(0, 1000);
            foreach(byte[] buffer in buffers)
            {
                int offset = 0;
                clientEngine.objectInstantManager.Deserialize(buffer, ref offset);
            }
        }
    }
}
