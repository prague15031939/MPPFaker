using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml.Serialization;
using FakerDTO;
using System.Data.Common;

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
            config.Add<StructExample, int, GacGenerator>(ex => ex.d);
            var faker = new Faker(config);
            Example obj = faker.Create<Example>();
            //Console.WriteLine(new CustomXmlSerializer().Serialize(obj));
            Console.ReadKey();
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
        public string name { get; set; }
        private int num;
        public int numa { get; set; }
        public double doubleItem { get; set; }
        public StructExample dadadada { get; set; }

        private Example2(string name, int num, double doubleItem, int numa)
        {
            this.name = name;
            this.num = 13;
            this.numa = 42;
            this.doubleItem = 56.23;
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
        public dad dadwdwfa;
    }
 
    public class dad
    {
        public dad()
        {
            int y = 0;
            y = 9 / y;
        }

        public int sho;
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
            return "PopGeneratorString";
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
