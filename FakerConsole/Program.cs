using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FakerDTO;

namespace FakerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var faker = new Faker();
            Example obj = faker.Create<Example>();
            Console.WriteLine("int property: {0}, double property: {1}, string property: {2}", obj.IntNumber, obj.DoubleNumber, obj.StringItem);
            //AssosiationExample obj = faker.Create<AssosiationExample>();
            //Console.WriteLine("int property: {0}, double property: {1}, string property: {2}; {3}", obj.DTOItem.IntNumber, obj.DTOItem.DoubleNumber, obj.DTOItem.StringItem, obj.HigherInt);

            Console.ReadKey();
        }
    }

    public class Example
    {
        public string StringItem { get; set; }
        public int IntNumber { get; set; }
        public double DoubleNumber { get; set; }

        public AnotherAssosiationExample assosiation;
    }

    public class AssosiationExample
    {
        //public Example DTOItem;
        public int HigherInt { get; set; }
    }

    public class AnotherAssosiationExample
    {
        public AssosiationExample AnotherDTOItem;
    }
}
