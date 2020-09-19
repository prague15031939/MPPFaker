using System;

using FakerDTO;

namespace IntGeneratorPlugin
{
    [Name(GeneratorType = "System.Int32")]
    public class IntGenerator : IGenerator
    {
        private Random _random;

        public IntGenerator()
        {
            _random = new Random();
        }

        public object Generate()
        {
            return _random.Next(100);
        }
    }
}
