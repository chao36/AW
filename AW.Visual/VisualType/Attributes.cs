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
        public string SourceName { get; set; }
        public string UpdateSourceEventName { get; set; }

        public AWComboBoxAttribute(int index = 0, string tag = null, string sourceName = null, string updateSourceEventName = null) : base(index, tag)
        {
            SourceName = sourceName;
            UpdateSourceEventName = updateSourceEventName;
        }
    }
}
  