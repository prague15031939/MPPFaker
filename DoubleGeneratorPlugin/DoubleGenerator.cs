using System;

using FakerDTO;

namespace DoubleGeneratorPlugin
{
    [Name(GeneratorType = "System.Double")]
    public class DoubleGenerator : IGenerator
    {
        private Random _random;

        public DoubleGenerator()
        {
            _random = new Random();
        }

        public object Generate()
        {
            return _random.NextDouble() * 113;
        }
    }
}
