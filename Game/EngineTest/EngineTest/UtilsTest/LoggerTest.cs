using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyGame.Engine.Utils;

namespace EngineTest.EngineTest.UtilsTest
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Logger.StartLogger();
            Logger log = new Logger(this.GetType());
            log.Info("This is a test");
            log.Error("Thsdfsdfsdfsfsdfsdfsdfefsfdfefsdfsdfsdfsef");
            Logger.JoinWriter();
        }
    }
}
