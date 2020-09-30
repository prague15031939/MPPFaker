using System;
using System.Collections.Generic;
using System.Reflection;

namespace FakerDTO
{
    class CMF
    {
        private Stack<List<MemberInfo>> ConstructorFilledMembers = new Stack<List<MemberInfo>>();

        public void NewConstructor()
        {
            ConstructorFilledMembers.Push(new List<MemberInfo>());
        }

        public void AddMember(MemberInfo member)
        {
            var list = ConstructorFilledMembers.Pop();
            if (!list.Contains(member))
                list.Add(member);
            ConstructorFilledMembers.Push(list);
        }

        public bool isMemberFilled(MemberInfo member)
        {
            var list = ConstructorFilledMembers.Peek();
            if (list.Contains(member))
                return true;
            else
                return false;
        }

        public void DestroyConstructor()
        {
            ConstructorFilledMembers.Pop();
        }
    }
}
