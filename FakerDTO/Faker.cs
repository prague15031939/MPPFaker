using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace FakerDTO
{
    public class Faker
    {
        internal Dictionary<Type, IGenerator> generators = null;
        internal FakerConfig config = null;
        internal CircularReferencesDodger dodger = new CircularReferencesDodger(3);

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
            return (T)CreateByType(typeof(T));
        }

        internal dynamic CreateByType(Type TargetType)
        {
            if (generators.ContainsKey(TargetType))
                return generators[TargetType].Generate();
            if ((TargetType.IsValueType || TargetType == typeof(string)) && !isStruct(TargetType))
                return GetDefault(TargetType);

            List<ConstructorInfo> TypeCtors = TargetType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToList();
            object TargetObject = CustomCreateInstance(TargetType, TypeCtors);
            if (TargetObject == null) return GetDefault(TargetType);
            dodger.AddReference(TargetType);
            MemberInfo[] members = TargetType.GetMembers();

            foreach (MemberInfo member in members)
            {
                if (isMemberFilled(TargetObject, member)) continue;

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
            return TargetObject;
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
                object SubObject = InvokeConfigGeneration(key);
                AssignValue(target, member, SubObject, mType);
                return;
            }

            if (isDTO(ObjectType))
            {
                object SubObject = null;
                if (dodger.CanRecurse(ObjectType))
                    SubObject = CreateByType(ObjectType);

                AssignValue(target, member, SubObject, mType);
                return;
            }

            if (isGenericList(ObjectType))
            {
                var ListObject = new ListGenerator(this, ObjectType).Generate();
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

        private object CustomCreateInstance(Type TargetType, List<ConstructorInfo> TypeCtors)
        {
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
                    obj = CreateByType(param.ParameterType);
                else
                {
                    foreach (var key in config.SettingsDict.Keys)
                    {
                        if (key.ClassType == TargetType && isCtorParameterFit(key, param))
                        {
                            obj = InvokeConfigGeneration(key);
                            break;
                        }
                    }
                }

                if (obj == null)
                    obj = CreateByType(param.ParameterType);
                сtorParams.Add(obj);
            }

            try
            {
                return Activator.CreateInstance(TargetType, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, сtorParams.ToArray(), null);
            }
            catch
            {
                TypeCtors.Remove(ctor);
                if (TypeCtors.Count != 0)
                    return CustomCreateInstance(TargetType, TypeCtors);
                else
                    return null;
            }

        }

        private object InvokeConfigGeneration((Type ClassType, MemberInfo member) key)
        {
            Type GeneratorType = config.SettingsDict[key];
            var generator = Activator.CreateInstance(GeneratorType);
            return (generator as IGenerator).Generate();
        }

        public bool isMemberFilled(object TargetObject, MemberInfo member)
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

        public object GetDefault(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            else
                return null;
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
            return (!(isGenericList(type) || type.IsValueType || type == typeof(string)) || isStruct(type));
        }

        public static bool isStruct(Type type)
        {
            return (type.IsValueType && type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Length != 0);
        }

        private static bool isGenericList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
