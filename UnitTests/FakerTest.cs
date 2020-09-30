using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FakerDTO;
using System.Configuration;

namespace UnitTests
{
    [TestClass]
    public class FakerTest
    {
        public Faker faker;

        [TestInitialize]
        public void Setup()
        {
            faker = new Faker();
        }

        [TestMethod]
        public void TestSimpleClass()
        {
            SimpleClass obj = faker.Create<SimpleClass>();
            Assert.IsTrue(obj.IntNumber > 0 && obj.IntNumber <= 1000);
            Assert.IsTrue(obj.StringItem != null);
            Assert.IsTrue(obj.DoubleNumber > 0 && obj.DoubleNumber <= 113);
        }

        [TestMethod]
        public void TestRestrictedMembers()
        {
            RestrictedMembersClass obj = faker.Create<RestrictedMembersClass>();
            Assert.AreEqual(obj.StringItem, null);
            Assert.AreEqual(obj.DoubleNumber, 0);
        }

        [TestMethod]
        public void TestDTOMembers()
        {
            DTOMembersClass obj = faker.Create<DTOMembersClass>();
            Assert.IsTrue(obj.rmc != null && obj.sc != null);
            Assert.IsTrue(obj.sc.IntNumber > 0 && obj.sc.IntNumber <= 1000);
            Assert.AreEqual(obj.rmc.StringItem, null);
            Assert.IsTrue(obj.rmc.IntNumber > 0 && obj.rmc.IntNumber <= 1000);
        }

        [TestMethod]
        public void TestGenericList()
        {
            GenericListClass obj = faker.Create<GenericListClass>();
            Assert.IsTrue(obj.IntList != null && obj.StringList != null && obj.DTOList != null);
            Assert.IsTrue(obj.IntList.Count != 0 && obj.IntList[0] > 0 && obj.IntList[0] <= 1000);
            Assert.IsTrue(obj.StringList[0] is String && obj.StringList[0].Length != 0);
            Assert.IsTrue(obj.DTOList.Count != 0 && obj.DTOList[0].DoubleNumber != 0);
        }

        [TestMethod]
        public void TestCircularReferences()
        {
            CircularReferencesClass obj = faker.Create<CircularReferencesClass>();
            Assert.AreEqual(obj.rec.rec.rec, null, "failed on wrapping");
            Assert.IsTrue(obj.rec.IntNumber != 0 && obj.rec.rec.IntNumber != 0);
        }
        
        [TestMethod]
        public void TestListCircularReferances()
        {
            ListCircularReferencesClass obj = faker.Create<ListCircularReferencesClass>();
            Assert.AreNotEqual(obj.rec[0].rec, null);
            Assert.AreEqual(obj.rec[0].rec[0].rec.Count, 0, "failed on wrapping");
            Assert.IsTrue(obj.rec[0].rec[0].IntNumber > 0 && obj.rec[0].rec[0].IntNumber < 1000);
        }

        [TestMethod]
        public void TestDateTimeClass()
        {
            DateTime obj = faker.Create<DateTime>();
            Assert.AreNotEqual(obj.Ticks, 0);
        }

        [TestMethod]
        public void TestPureTypes()
        {
            TestEnum obj1 = faker.Create<TestEnum>();
            Assert.AreEqual(obj1, TestEnum.item1);
            int obj2 = faker.Create<int>();
            Assert.IsTrue(obj2 > 0 && obj2 <= 1000);
            string obj3 = faker.Create<string>();
            Assert.IsTrue(obj3 != null && obj3.Length != 0 && (obj3[0] >= 'a' && obj3[0] <= 'z' || obj3[0] >= 'A' && obj3[0] <= 'Z'), "failed on string");
        }

        [TestMethod]
        public void TestCtorCreation()
        {
            // also checks for filling private members
            Example2 obj = faker.Create<Example2>();
            Assert.AreNotEqual(obj.doubleItem, 0);
            Assert.IsTrue(obj.name != null && obj.name.Length != 0);
            Assert.IsTrue(obj.numa > 0 && obj.numa <= 1000);
        }

        [TestMethod]
        public void TestStructCreation()
        {
            StructExample obj = faker.Create<StructExample>();
            Assert.AreEqual(obj.a, "yes");
            Assert.IsTrue(obj.d > 0 && obj.d <= 1000);
        }
    }

    public class SimpleClass
    {
        public string StringItem { get; set; }
        public int IntNumber { get; set; }
        public double DoubleNumber;
    }

    public class RestrictedMembersClass
    {
        public string StringItem { get; }
        public int IntNumber { get; set; }
        public double DoubleNumber { get; private set; }
    }

    public class DTOMembersClass
    {
        public string StringItem { get; set; }
        public RestrictedMembersClass rmc;
        public SimpleClass sc;
    }

    public class GenericListClass
    {
        public List<int> IntList;
        public List<string> StringList;
        public List<SimpleClass> DTOList;
    }

    public class CircularReferencesClass
    {
        public int IntNumber;
        public CircularReferencesClass rec { get; set; }
    }

    public class ListCircularReferencesClass
    {
        public int IntNumber;
        public List<ListCircularReferencesClass> rec;
    }

    public enum TestEnum
    {
        item1, item2, item3
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
}
