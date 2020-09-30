using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FakerDTO;

namespace UnitTests
{
    [TestClass]
    public class ConfigFakerTest
    {
        public Faker faker;

        [TestInitialize]
        public void Setup()
        {
            var config = new FakerConfig();
            config.Add<Foo, string, ShoGenerator>(ex => ex.DeterminedString);
            config.Add<Example, int, GACGenerator>(ex => ex.IntNumber);
            config.Add<Example2, string, PopGenerator>(ex => ex.name);
            config.Add<Example2, int, GACGenerator>(ex => ex.numa);
            config.Add<StructExample, int, GACGenerator>(ex => ex.d);
            faker = new Faker(config);
        }

        [TestMethod]
        public void TestSimpleConfig()
        {
            Foo foo = faker.Create<Foo>();
            Assert.AreEqual(foo.DeterminedString, "ShoGeneratorString");
            Assert.AreNotEqual(foo.VolatileString, "ShoGeneratorString");
        }

        [TestMethod]
        public void TestCtorConfig()
        {
            Example2 obj = faker.Create<Example2>();
            Assert.AreEqual(obj.name, "PopGeneratorString");
            Assert.AreEqual(obj.numa, 42);
        }

        [TestMethod]
        public void TestComplexClass()
        {
            Example obj = faker.Create<Example>();
            Assert.AreEqual(obj.IntNumber, 42);
            Assert.AreEqual(obj.Example2Item.name, "PopGeneratorString");
            Assert.AreEqual(obj.Example2Item.numa, 42);
            Assert.AreEqual(obj.LongNumber, 0);
            Assert.IsTrue(obj.ListAssosItem != null && obj.ListAssosItem.Count != 0);
            Assert.AreEqual(obj.ListAssosItem[0].DTOItem.ListAssosItem[0].DTOItem.ListAssosItem[0].DTOItem, null, "failed on wrapping");
            Assert.IsTrue(obj.ListAssosItem[0].HigherInt > 0 && obj.ListAssosItem[0].HigherInt <= 1000);
            Assert.AreEqual(obj.ListAssosItem[0].DTOItem.ListAssosItem[0].DTOItem.Example2Item.name, "PopGeneratorString");
            Assert.IsTrue(obj.DateTimeItem.Ticks != 0);
            Assert.AreEqual(obj.strct.a, "yes");
        }
    }

    public class Foo
    {
        public string DeterminedString { get; set; }
        public string VolatileString;
        public Example2 ex;
    }

    public class Example
    {
        public List<int> ListIntItem { get; set; }
        public List<string> ListStringItem;
        public List<AssosiationExample> ListAssosItem { get; set; }
        public string StringItem { get; set; }
        public int IntNumber { get; set; }
        public double DoubleNumber { get; set; }
        public long LongNumber;
        public DateTime DateTimeItem;
        public Example2 Example2Item;
        public StructExample strct;
    }

    public class AssosiationExample
    {
        public Example DTOItem;
        public int HigherInt { get; set; }
    }

    public class ShoGenerator : IGenerator
    {
        public object Generate()
        {
            return "ShoGeneratorString";
        }
    }

    public class PopGenerator : IGenerator
    {
        public object Generate()
        {
            return "PopGeneratorString";
        }
    }

    public class GACGenerator : IGenerator
    {
        public object Generate()
        {
            return 42;
        }
    }
}
