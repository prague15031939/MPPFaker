using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FakerDTO;

namespace UnitTests
{
    [TestClass]
    public class FakerTest
    {
        [TestInitialize]
        public void Setup()
        {
            var config = new FakerConfig();
            config.Add<Example, string, ShoGenerator>(ex => ex.StringItem);
            config.Add<Example, int, GacGenerator>(ex => ex.IntNumber);
            config.Add<Example2, string, PopGenerator>(ex => ex.name);
            config.Add<StructExample, int, GacGenerator>(ex => ex.d);
            var faker = new Faker(config);
        }

        [TestMethod]
        public void TestThreadCount()
        {
        }
    }

    public struct StructExample
    {
        public StructExample(int d, string a)
        {
            this.d = d;
            this.a = "yes";
        }

        public int d;
        public string a;
    }

    public class Example2
    {
        public string name { get; }
        private int num;
        public int numa { get; }
        public double doubleItem { get; set; }
        public StructExample da;

        private Example2(string name, int num, double doubleItem, int numa)
        {
            this.name = name;
            this.num = num;
            this.numa = numa;
            this.doubleItem = doubleItem;
        }

        public Example2(string name)
        {
            this.name = name;
        }
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
    }

    public class AssosiationExample
    {
        public Example DTOItem;
        public int HigherInt { get; set; }
    }

    public class AnotherAssosiationExample
    {
        public AssosiationExample AnotherDTOItem;
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
            return "AAaaaPaA";
        }
    }

    public class GacGenerator : IGenerator
    {
        public object Generate()
        {
            return 42;
        }
    }
}
