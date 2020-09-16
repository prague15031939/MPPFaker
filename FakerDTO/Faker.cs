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
        internal Dictionary<Type, IGenerator> _generators = new Dictionary<Type, IGenerator>();

        private enum WorkingMemberType
        {
            property, field,
        }

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
                        FillPropertyOrField(TargetObject, member, ((PropertyInfo)member).PropertyType, WorkingMemberType.property);
                    }
                }
                else if (member.MemberType == MemberTypes.Field && ((FieldInfo)member).IsPublic)
                {
                    FillPropertyOrField(TargetObject, member, ((FieldInfo)member).FieldType, WorkingMemberType.field);
                }
            }

            return (T)TargetObject;
        }

        private void AssignValue(object target, MemberInfo member, object value, WorkingMemberType mType)
        {
            if (mType == WorkingMemberType.property)
                (member as PropertyInfo).SetValue(target, value);
            else if (mType == WorkingMemberType.field)
                (member as FieldInfo).SetValue(target, value);
        }

        private void FillPropertyOrField(object target, MemberInfo member, Type ObjectType, WorkingMemberType mType)
        {
            if (isDTO(ObjectType))
            {
                var SubObject = InvokeCreation(ObjectType, "Create", this);
                AssignValue(target, member, SubObject, mType);
                return;
            }

            if (isGenericList(ObjectType))
            {
                Type ListElementType = ObjectType.GetGenericArguments().Single();
                var ListObject = InvokeCreation(ListElementType, "Generate", new ListGenerator());
                AssignValue(target, member, ListObject, mType);
                return;
            }

            if (_generators.ContainsKey(ObjectType))
            {
                object value = _generators[ObjectType].Generate();
                AssignValue(target, member, value, mType);
            }
            else
            {
                AssignValue(target, member, null, mType);
            }
        }

        private object InvokeCreation(Type MemberType, string MethodName, object PullingObject)
        {
            var TypeOfContext = PullingObject.GetType();
            var method = TypeOfContext.GetMethod(MethodName);
            var GenericMethod = method.MakeGenericMethod(MemberType);
            return GenericMethod.Invoke(PullingObject, null);
        }

        public bool isDTO(Type type)
        {
            return type.Namespace == "FakerConsole";
        }

        private static bool isGenericList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }
    }

}
