using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerDTO
{
    public class Faker
    {
        public T Create<T>()
        {
            Type TargetType = typeof(T);
            var TargetObject = Activator.CreateInstance(TargetType);
            MemberInfo[] members =  TargetType.GetMembers();
            foreach (MemberInfo member in members)
            {
                if (member.MemberType == MemberTypes.Property)
                {
                    if (((PropertyInfo)member).PropertyType == typeof(int))
                        ((PropertyInfo)member).SetValue(TargetObject, 2);
                    else if (((PropertyInfo)member).PropertyType == typeof(double))
                        ((PropertyInfo)member).SetValue(TargetObject, 13.42);
                    else if (((PropertyInfo)member).PropertyType == typeof(string))
                        ((PropertyInfo)member).SetValue(TargetObject, "example string");
                }
                else
                {
                    ;
                }
            }

            return (T)TargetObject;
        }
    }
}
