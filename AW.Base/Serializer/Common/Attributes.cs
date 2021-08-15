using System;

namespace AW
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AWSerializableAttribute : Attribute { }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class AWIgnoreAttribute : Attribute { }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class AWReferenceAttribute : Attribute { }
}
