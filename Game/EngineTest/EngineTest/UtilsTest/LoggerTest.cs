﻿using System;
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
            Logger log = new Logger(this.GetType());
            Logger.JoinWriter();
        }
    }
}
