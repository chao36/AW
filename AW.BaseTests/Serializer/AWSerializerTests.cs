using System;
using System.Collections.Generic;

using AW.Base.Serializer.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AW.Base.Serializer.Tests
{
    [TestClass()]
    public class AWSerializerTests
    {
        [Common.AWSerializable]
        public class Test
        {
            public DateTime Date { get; set; } = DateTime.Now;
            public double D { get; set; } = 2.09;

            public List<int> LI { get; set; } = new List<int>
            {
                1, 2, 3, 4, 5
            };

            public int[] AI { get; set; } = new int[2] { 1, 2 };

            public List<string> LS { get; set; } = new List<string>
            {
                "222", "asasa", "dwww"
            };

            public Dictionary<string, string> DS { get; set; } = new Dictionary<string, string>
            {
                { "s1", "asas" }, { "s2", "asasas" }, { "s3", "aghghsas" }
            };
        }

        [TestMethod()]
        public void SerializeTest()
        {
            Test test = new Test();
            string data = null;

            using (AWSerializer serializer = new AWSerializer())
            {
                data = serializer.Serialize(test);
            }

            test = null;

            using (AWSerializer serializer = new AWSerializer())
            {
                test = serializer.Deserialize<Test>(data);
            }

            Assert.IsTrue(test != null);
        }

        [TestMethod()]
        public void SerializeToFileTest()
        {
            Test test = new Test();
            string data = null;

            using (AWSerializer serializer = new AWSerializer())
            {
                data = serializer.Serialize(test);
            }

            SerializerHelper.SaveText(data, "fileName");

            data = null;
            test = null;

            data = SerializerHelper.LoadText("fileName");

            using (AWSerializer serializer = new AWSerializer())
            {
                test = serializer.Deserialize<Test>(data);
            }

            Assert.IsTrue(test != null);
        }

        [AWSerializable]
        public class Reference : IReference
        {
            public int ReferenceId { get; set; }
        }

        [AWSerializable]
        public class TestReference
        {
            public Reference F1 { get; set; } = new Reference();

            [AWReference]
            public List<Reference> References { get; set; } = new List<Reference>();

            public TestReference()
            {
                References.Add(F1);
                References.Add(F1);
                References.Add(F1);
                References.Add(F1);
            }
        }
    }
}