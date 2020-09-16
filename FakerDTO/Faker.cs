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
        private Dictionary<Type, IGenerator> _generators = new Dictionary<Type, IGenerator>();

        public Faker()
        {
            _generators.Add(typeof(int), new IntGenerator());
            _generators.Add(typeof(double), new DoubleGenerator());
            _generators.Add(typeof(string), new StringGenerator());
        }

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

            if (_generators.ContainsKey(property.PropertyType))
            {
                object value = _generators[property.PropertyType].Generate();
                property.SetValue(target, value);
            }
            else
                property.SetValue(target, null);

                /*else if (property.PropertyType == typeof(List<object>))
                {
                    var ListObject = new List<object>();
                    Type ListElementsType = ListObject.GetType().GetGenericArguments().Single();
                    for (int i = 0; i < 10; i++)
                    {

                    }
                    property.SetValue(target, ListObject);
                }*/
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

            if (_generators.ContainsKey(field.FieldType))
            {
                object value = _generators[field.FieldType].Generate();
                field.SetValue(target, value);
            }
            else
                field.SetValue(target, null);
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
