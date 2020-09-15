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
                    MethodInfo SetMethod = ((PropertyInfo)member).GetSetMethod();
                    if (SetMethod != null)
                    { 
                        FillProperty(TargetObject, (PropertyInfo)member);
                    }
                }
                else if (member.MemberType == MemberTypes.Field && ((FieldInfo)member).IsPublic)
                {
                    FillField(TargetObject, (FieldInfo)member);
                }
            }

            return (T)TargetObject;
        }

        private void FillProperty(object target, PropertyInfo property)
        {
            if (isDTO(property))
            {
                Type PropertyType = property.PropertyType;
                var SubObject = InvokeSubObjectCreation(PropertyType);
                property.SetValue(target, SubObject);
                return;
            }

            if (property.PropertyType == typeof(int))
                property.SetValue(target, 2);
            else if (property.PropertyType == typeof(double))
                property.SetValue(target, 13.42);
            else if (property.PropertyType == typeof(string))
                property.SetValue(target, "example property string");
        }

        private void FillField(object target, FieldInfo field) 
        {
            if (isDTO(field))
            {
                Type FieldType = field.FieldType;
                var SubObject = InvokeSubObjectCreation(FieldType);
                field.SetValue(target, SubObject);
                return;
            }

            if (field.FieldType == typeof(int))
                field.SetValue(target, 4);
            else if (field.FieldType == typeof(double))
                field.SetValue(target, 69.13);
            else if (field.FieldType == typeof(string))
                field.SetValue(target, "example field string");
        }

        private bool isDTO(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Property && ((PropertyInfo)member).PropertyType.Namespace == "FakerConsole" ||
                member.MemberType == MemberTypes.Field && ((FieldInfo)member).FieldType.Namespace == "FakerConsole")
                return true;
            return false;
        }

        private object InvokeSubObjectCreation(Type MemberType)
        {
            var TypeOfContext = GetType();
            var method = TypeOfContext.GetMethod("Create");
            var GenericMethod = method.MakeGenericMethod(MemberType);
            return GenericMethod.Invoke(this, null);
        }
    }
}
