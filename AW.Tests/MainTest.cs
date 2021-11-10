using AW.LangSupport;
using AW.Serializer;

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
        public void SerializerTest()
        {
            var test = new TestClass
            {
                Test0 = 2,
                Test1 = "2",
                Tests = new string[1] { "2" }
            };

            var data = Serializer.Serialize(test);
            //var data = test.Serialize();

            var parse = Serializer.Deserialize<TestClass>(data);
            //var parse = data.Deserialize<TestClass>();

            Assert.IsTrue(test.Test0 == parse.Test0 && test.Test1 == parse.Test1 && test.Tests[0] == parse.Tests[0]);
        }

        [Test]
        public void LangTest()
        {
            var config = new LangConfig();

            var lang = config.AddLang("lang");
            var word = config.AddWord("test");

            config.SetValue(word, lang, "Value 1");

            Assert.IsTrue("test".Translate() == "Value 1");
        }
    }
}