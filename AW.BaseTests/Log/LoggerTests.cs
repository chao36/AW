using Microsoft.VisualStudio.TestTools.UnitTesting;
using AW.Base.Log;

using System;
using System.Collections.Generic;
using System.Text;

namespace AW.Base.Log.Tests
{
    [TestClass()]
    public class LoggerTests
    {
        [TestMethod()]
        public void LogTest()
        {
            Logger.Log("test");
        }
    }
}