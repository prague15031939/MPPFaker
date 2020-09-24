using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace FakerDTO
{
    public class Faker
    {
        internal static Dictionary<Type, IGenerator> generators = null;
        internal static FakerConfig config = null;
        internal static CircularReferencesDodger dodger = new CircularReferencesDodger(3);

        private enum WorkingMemberType
        {
            property, field,
        }

        public Faker(FakerConfig cfg = null)
        {
            if (config == null) config = cfg;
            if (generators == null)
            {
                var loader = new PluginLoader();
                generators = loader.RefreshPlugins();
                generators.Add(typeof(string), new StringGenerator());
                generators.Add(typeof(DateTime), new DateTimeGenerator());
            }
        }

        public T Create<T>()
        {
            Type TargetType = typeof(T);
            if (generators.ContainsKey(TargetType))
                return (T)generators[TargetType].Generate();
            if (TargetType.IsValueType || TargetType == typeof(string))
            {
                object obj = null;
                return (T)obj;
            }

            object TargetObject = CustomCreateInstance(TargetType);
            dodger.AddReference(TargetType);
            MemberInfo[] members = TargetType.GetMembers();

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

            dodger.RemoveReference(TargetType);
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
            var key = (target.GetType(), member);
            if (config != null && config.SettingsDict.ContainsKey(key))
            {
                object SubObject = InvokeGeneration(key);
                AssignValue(target, member, SubObject, mType);
                return;
            }

            if (isDTO(ObjectType))
            {
                object SubObject = null;
                if (dodger.CanRecurse(ObjectType))
                    SubObject = InvokeCreation(ObjectType, "Create", this);

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

            if (generators.ContainsKey(ObjectType))
            {
                object value = generators[ObjectType].Generate();
                AssignValue(target, member, value, mType);
            }
            else
            {
                AssignValue(target, member, null, mType);
            }
        }

        private object CustomCreateInstance(Type TargetType) 
        {
            ConstructorInfo[] TypeCtors = TargetType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            if (TypeCtors.Length == 0)
                  return Activator.CreateInstance(TargetType);

            ConstructorInfo ctor = TypeCtors[0];
            foreach (ConstructorInfo item in TypeCtors)
            {
                if (item.GetParameters().Length > ctor.GetParameters().Length)
                    ctor = item;
            }

            var сtorParams = new List<object>();
            foreach (ParameterInfo param in ctor.GetParameters())
            {
                object obj = null;
                if (config == null)
                    obj = InvokeCreation(param.ParameterType, "Create", this);
                else
                {
                    foreach (var key in config.SettingsDict.Keys)
                    {
                        if (key.ClassType == TargetType && isCtorParameterFit(key, param))
                        {
                            obj = InvokeGeneration(key);
                            break;
                        }
                    }
                }
                
                if (obj == null)
                    obj = InvokeCreation(param.ParameterType, "Create", this); 
                сtorParams.Add(obj);
            }

            return Activator.CreateInstance(TargetType, BindingFlags.NonPublic | BindingFlags.Instance, null, сtorParams.ToArray(), null);
        }

        private object InvokeCreation(Type MemberType, string MethodName, object PullingObject)
        {
            var TypeOfContext = PullingObject.GetType();
            var method = TypeOfContext.GetMethod(MethodName);
            var GenericMethod = method.MakeGenericMethod(MemberType);
            return GenericMethod.Invoke(PullingObject, null);
        }

        private object InvokeGeneration((Type ClassType, MemberInfo member) key)
        {
            Type GeneratorType = config.SettingsDict[key];
            var generator = Activator.CreateInstance(GeneratorType);
            MethodInfo GenerateMethod = GeneratorType.GetMethod("Generate");
            return GenerateMethod.Invoke(generator, null);
        }

        private bool isCtorParameterFit((Type ClassType, MemberInfo member) key, ParameterInfo param)
        {
            if (key.member.MemberType == MemberTypes.Field && 
                (key.member as FieldInfo).FieldType == param.ParameterType && (key.member as FieldInfo).Name.ToLower() == param.Name.ToLower() || 
                key.member.MemberType == MemberTypes.Property && (key.member as PropertyInfo).PropertyType == param.ParameterType && 
                (key.member as PropertyInfo).Name.ToLower() == param.Name.ToLower()) 
                return true;
            return false;
        }

        public static bool isDTO(Type type)
        {
            return !(isGenericList(type) || 
                    type.IsValueType || 
                    type == typeof(string));
        }

        private static bool isGenericList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
