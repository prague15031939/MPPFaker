using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FakerDTO
{
    public class NameAttribute : Attribute
    {
        public string GeneratorType;
    }

    public interface IGenerator
    {
        dynamic Generate();
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

    public class ListGenerator : IGenerator
    {
        private Random _random;
        private Faker faker;
        private Type ObjectType;

        public ListGenerator(Faker faker, Type ObjectType)
        {
            _random = new Random();
            this.faker = faker;
            this.ObjectType = ObjectType;
        }

        public object Generate()
        {
            Type ItemsType = ObjectType.GetGenericArguments().Single();
            dynamic list = Activator.CreateInstance(ObjectType);

            int ListLength = _random.Next(1, 21);
            if (Faker.isDTO(ItemsType))
            {
                if (faker.dodger.CanRecurse(ItemsType))
                {
                    for (int i = 0; i < ListLength; i++)
                        list.Add(faker.CreateByType(ItemsType));
                }
            }
            else
            {
                if (faker.generators.ContainsKey(ItemsType))
                {
                    IGenerator generator = faker.generators[ItemsType];
                    for (int i = 0; i < ListLength; i++)
                        list.Add(generator.Generate());
                }
            }

            return list;
        }
    }
}
