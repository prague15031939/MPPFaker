using System;
using System.Collections.Generic;
using System.Reflection;

namespace FakerDTO
{
    class ConstructorFilledMembers
    {
        public static bool isMemberFilled(object TargetObject, MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    {
                        object TargetValue = ((PropertyInfo)member).GetValue(TargetObject);
                        object DefaultValue = GetDefault(((PropertyInfo)member).PropertyType);
                        return !Equals(DefaultValue, TargetValue);
                    }
                case MemberTypes.Field:
                    {
                        object TargetValue = ((FieldInfo)member).GetValue(TargetObject);
                        object DefaultValue = GetDefault(((FieldInfo)member).FieldType);
                        return !Equals(DefaultValue, TargetValue);
                    }
            }

            return false;
        }

        public static object GetDefault(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            else
                return null;
        }

    }
}
