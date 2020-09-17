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
            _random = new Random();
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
            _random = new Random();
        }

        public object Generate()
        {
            return _random.NextDouble() * 113;
        }
    }

    public class StringGenerator : IGenerator
    {
        private Random _random;

        public StringGenerator()
        {
            _random = new Random();
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

    public class ListGenerator
    {
        private Random _random;

        public ListGenerator()
        {
            _random = new Random();
        }

        public object Generate<T>()
        {
            int ListLength = _random.Next(20);
            var list = new List<T>();
            var faker = new Faker();

            if (faker.isDTO(typeof(T)))
            {
                if (Faker.dodger.CanRecurse(typeof(T)))
                {
                    for (int i = 0; i < ListLength; i++)
                        list.Add(faker.Create<T>());
                }
            }
            else
            {
                IGenerator generator = faker.generators[typeof(T)];
                for (int i = 0; i < ListLength; i++)
                    list.Add((T)generator.Generate());
            }

            return list;
        }
    }
}
