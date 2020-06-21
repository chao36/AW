using System;

namespace AW.Visual.VisualType
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AWPropertyAttribute : Attribute
    {
        public int Index { get; }

        public AWPropertyAttribute(int index = 0)
            => Index = index;
    }

    public class AWReadonlyAttribute : Attribute { }
}
