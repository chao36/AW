using System;
using System.Collections.Generic;

namespace AW.Base
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AWPropertyAttribute : Attribute
    {
        public int Index { get; }
        public string Tag { get; }

        public AWPropertyAttribute(int index = 0, string tag = null)
        {
            Index = index;
            Tag = tag;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AWNumberAttribute : AWPropertyAttribute
    {
        public IEnumerable<string> AllowedStrings { get; }

        public AWNumberAttribute(int index = 0, string tag = null, IEnumerable<string> allowedStrings = null) : base(index, tag)
            => AllowedStrings = allowedStrings;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AWStringAttribute : AWNumberAttribute
    {
        public int MaxLength { get; }

        public AWStringAttribute(int index = 0, string tag = null, int maxLength = 0, IEnumerable<string> allowedStrings = null)
            : base(index, tag, allowedStrings) 
            => MaxLength = maxLength;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AWReadonlyAttribute : AWPropertyAttribute
    {
        public AWReadonlyAttribute(int index = 0, string tag = null) : base(index, tag) { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AWComboBoxAttribute : AWPropertyAttribute
    {
        public string DisplayMemberPath { get; }
        public string SourceName { get; }
        public string UpdateSourceEventName { get; }

        public AWComboBoxAttribute(int index = 0, string tag = null, string displayMemberPath = null, string sourceName = null, string updateSourceEventName = null) : base(index, tag)
        {
            DisplayMemberPath = displayMemberPath;
            SourceName = sourceName;
            UpdateSourceEventName = updateSourceEventName;
        }
    }
}
