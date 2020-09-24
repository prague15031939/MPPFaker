using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FakerDTO
{
    public class FakerConfig
    {
        internal Dictionary<(Type ClassType, MemberInfo member), Type> SettingsDict = new Dictionary<(Type, MemberInfo), Type>();

        public void Add<TClass, TMember, TGenerator>(Expression<Func<TClass, TMember>> expr)
        {
            MemberExpression operation = (MemberExpression)expr.Body;
            MemberInfo member = operation.Member;

            var key = (typeof(TClass), member);
            if (!SettingsDict.ContainsKey(key))
                SettingsDict.Add(key, typeof(TGenerator));
        }
    }
}
