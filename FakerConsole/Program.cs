using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml.Serialization;
using FakerDTO;

namespace FakerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new FakerConfig();
            config.Add<Example, string, ShoGenerator>(ex => ex.StringItem);
            config.Add<Example, int, GacGenerator>(ex => ex.IntNumber);
            config.Add<Example2, string, PopGenerator>(ex => ex.name);
            var faker = new Faker(config);
            Example obj = faker.Create<Example>();
            //Console.WriteLine(new CustomXmlSerializer().Serialize(obj));
            Console.ReadKey();
        }
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

    public class CustomXmlSerializer
    {
        public string Serialize(object obj)
        {
            MemoryStream SerializationStream = new MemoryStream();
            XmlSerializer formatter = new XmlSerializer(typeof(Example));
            formatter.Serialize(SerializationStream, obj);
            return Encoding.Default.GetString(SerializationStream.ToArray());
        }
    }
}
