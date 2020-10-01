using System;
using System.Collections.Generic;

namespace FakerDTO
{
    public class NameAttribute : Attribute
    {
        public string GeneratorType;
    }

    public interface IGenerator
    {
        object Generate();
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
            int StringLength = _random.Next(1, 21);
            string str = String.Empty;
            for (int i = 0; i < StringLength; i++)
            {
                int register = _random.Next(0, 2);
                str += register == 0 ? (char)_random.Next(65, 91) : (char)_random.Next(97, 123);
            }
            return str;
        }
    }

    public class DateTimeGenerator : IGenerator
    {
        private Random _random;

        public DateTimeGenerator()
        {
            _random = new Random();
        }

        public object Generate()
        {
            int year = _random.Next(1, 2021);
            int month = _random.Next(1, 13);
            int day = _random.Next(1, 29);
            int hour = _random.Next(24);
            int minute = _random.Next(60);
            int second = _random.Next(60);
            return new DateTime(year, month, day, hour, minute, second);
        }
    }

    public class ListGenerator
    {
        private Random _random;
        private Faker faker;

        public ListGenerator(Faker faker)
        {
            _random = new Random();
            this.faker = faker;
        }

        public object Generate<T>()
        {
            int ListLength = _random.Next(1, 21);
            var list = new List<T>();

            if (Faker.isDTO(typeof(T)))
            {
                if (faker.dodger.CanRecurse(typeof(T)))
                {
                    for (int i = 0; i < ListLength; i++)
                        list.Add(faker.Create<T>());
                }
            }
            else
            {
                if (faker.generators.ContainsKey(typeof(T)))
                {
                    IGenerator generator = faker.generators[typeof(T)];
                    for (int i = 0; i < ListLength; i++)
                        list.Add((T)generator.Generate());
                }
            }

            return list;
        }
    }
}
