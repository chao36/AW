using System;

namespace AW.Visual.VisualType
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

        public AWComboBoxAttribute(int index = 0, string tag = null, string displayMemberPath, string sourceName = null, string updateSourceEventName = null) : base(index, tag)
        {
            DisplayMemberPath = displayMemberPath;
            SourceName = sourceName;
            UpdateSourceEventName = updateSourceEventName;
        }
    }
}
  