using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDTO
{
    public interface IGenerator
    {
        object Generate();
    }

    public class IntGenerator : IGenerator
    {
        private Random _random;

        public IntGenerator()
        {
            _random = new Random(1939);
        }

        public object Generate()
        {
            return _random.Next(100);
        }
    }

    public class DoubleGenerator : IGenerator
    {
        private Random _random;

        public DoubleGenerator()
        {
            _random = new Random(1938);
        }

        public object Generate()
        {
            return _random.NextDouble() * 100;
        }
    }

    public class StringGenerator : IGenerator
    {
        private Random _random;

        public StringGenerator()
        {
            _random = new Random(1995);
        }

        public object Generate()
        {
            int StringLength = _random.Next(20);
            string str = String.Empty;
            for (int i = 0; i < StringLength; i++)
            {
                int register = _random.Next(0, 2);
                str += register == 0 ? (char)_random.Next(65, 91) : (char)_random.Next(97, 123);
            }
            return str;
        }
    }
}
