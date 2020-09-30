using System;
using System.Collections.Generic;
using System.Reflection;

namespace FakerDTO
{
    public enum ConstructorDestroyMode
    {
        soft, hard
    }

    class CMF
    {
        private Stack<List<MemberInfo>> ConstructorFilledMembers = new Stack<List<MemberInfo>>();
        private bool anyMemberPeeked = false;

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
            anyMemberPeeked = true;
            var list = ConstructorFilledMembers.Peek();
            if (list.Contains(member))
                return true;
            else
                return false;
        }

        public void DestroyConstructor(ConstructorDestroyMode mode)
        {
            if (mode == ConstructorDestroyMode.hard)
                ConstructorFilledMembers.Pop();
            else if (mode == ConstructorDestroyMode.soft)
            {
                if (anyMemberPeeked)
                    ConstructorFilledMembers.Pop();
            }
        }
    }
}
