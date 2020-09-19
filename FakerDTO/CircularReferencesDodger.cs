using System;
using System.Collections.Generic;

namespace FakerDTO
{
    public class CircularReferencesDodger
    {
        private Dictionary<Type, int> references = new Dictionary<Type, int>();
        private int MaxRecursionLevel;

        public CircularReferencesDodger(int MaxRecursionLevel)
        {
            this.MaxRecursionLevel = MaxRecursionLevel;
        }

        public void AddReference(Type type)
        {
            if (references.ContainsKey(type))
                references[type] += 1;
            else
                references[type] = 1;
        }

        public void RemoveReference(Type type)
        {
            if (references.ContainsKey(type))
                references[type] -= 1;
        }

        public bool CanRecurse(Type type)
        {
            return !references.ContainsKey(type) || references[type] < MaxRecursionLevel;
        }

    }
}
