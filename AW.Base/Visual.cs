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
    public class AWActionAttribute : AWPropertyAttribute
    {
        public string Content { get; }
        public string CanExecuteName { get; }

        public AWActionAttribute(int index = 0, string tag = null, string content = null, string canExecuteName = null)
            : base(index, tag)
        {
            Content = content;
            CanExecuteName = canExecuteName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AWFilePathAttribute : AWPropertyAttribute
    {
        public bool OnlyFolder { get; }

        public string Filter { get; }
        public string Message { get; }

        public AWFilePathAttribute(int index = 0, string tag = null, string filter = null, string message = null, bool onlyFolder = false)
            : base(index, tag)
        {
            OnlyFolder = onlyFolder;

            Filter = filter;
            Message = message;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AWLimitAttribute : AWPropertyAttribute
    {
        public int MaxLength { get; }
        public IEnumerable<string> AllowedStrings { get; }

        public AWLimitAttribute(int index = 0, string tag = null, int maxLength = 0, IEnumerable<string> allowedStrings = null)
            : base(index, tag)
        {
            MaxLength = maxLength;
            AllowedStrings = allowedStrings;
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

        public AWComboBoxAttribute(int index = 0, string tag = null, string displayMemberPath = null, string sourceName = null, string updateSourceEventName = null) : base(index, tag)
        {
            DisplayMemberPath = displayMemberPath;
            SourceName = sourceName;
            UpdateSourceEventName = updateSourceEventName;
        }
    }
}
