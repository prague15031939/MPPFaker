﻿using System;
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
            config.Add<Example2, string, ShoGenerator>(ex => ex.name);
            var faker = new Faker(config);
            Example2 obj = faker.Create<Example2>();
            //Console.WriteLine(new CustomXmlSerializer().Serialize(obj));
            Console.ReadKey();
        }
    }

    public class Example2
    {
        public string name { get; set; }
        private int num;
        public double doubleItem { get; set; }

        private Example2(string name, int num, double doubleItem)
        {
            this.name = name;
            this.num = num;
            this.doubleItem = doubleItem;
        }
    }

    public class Example
    {
        public List<int> ListIntItem { get; set; }
        public List<string> ListStringItem;
        public List<Example2> ListAssosItem { get; set; }
        public string StringItem { get; set; }
        public int IntNumber { get; set; }
        public double DoubleNumber { get; set; }
        public long LongNumber;
        public DateTime DateTimeItem;
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
