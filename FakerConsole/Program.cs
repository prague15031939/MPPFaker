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

            Console.ReadKey();
        }
    }

    public class Example
    {
        public string StringItem { get; set; }
        public int IntNumber { get; set; }
        public double DoubleNumber { get; set; }
    }
}
