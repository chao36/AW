using AW;
using AW.Serializer;
using AW.LangSupport;

using NUnit.Framework;

namespace AW.Tests
{
    public class Tests
    {
        AWSerializer Serializer;


        [SetUp]
        public void Setup()
        {
            Serializer = new AWSerializer();
        }


        [AWSerializable]
        public class TestClass
        {
            public int Test0 { get; set; }
            public string Test1 { get; set; }

            public string[] Tests { get; set; }
        }


        [Test]
        public void Test1()
        {
            var test = new TestClass
            {
                Test0 = 2,
                Test1 = "2",
                Tests = new string[1] { "2" }
            };

            var data = Serializer.Serialize(test);

            var test2 = Serializer.Deserialize<TestClass>(data);

            Assert.IsTrue(test.Test0 == test2.Test0 && test.Test1 == test2.Test1 && test.Tests[0] == test2.Tests[0]);
        }


        [Test]
        public void Test2()
        {
            var config = new LangConfig();

            var l = config.AddLang("l1");
            var w = config.AddWord("w1");

            config.SetValue(w, l, "v1");

            config.SaveAsXmlResource("langs");
        }
    }
}