using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class ComboBoxControl : UserControl
    {
        public ComboBoxControl(bool hideTag, string displayMemberPath)
        {
            InitializeComponent();

            if (hideTag)
                TagLabel.Visibility = Visibility.Collapsed;

            Element.DisplayMemberPath = displayMemberPath;
        }
    }

    public class ComboBoxContext : VisualTypeContext
    {
        private Func<ObservableCollection<object>> GetSource;

        public ComboBoxContext(string tag, string placeholder, object source, string property, string displayMemberPath, string itemSourceName, string itemSourceEventUpdateName, bool? hideTag = null)
            : base(tag, source, property, new ComboBoxControl(hideTag ?? string.IsNullOrEmpty(tag), displayMemberPath))
        {
            if (!string.IsNullOrEmpty(itemSourceName))
            {
                Type type = source.GetType();

                PropertyInfo propertyInfo = type.GetProperty(itemSourceName);

                if (propertyInfo != null)
                    GetSource = () => new ObservableCollection<object>(propertyInfo.GetValue(source) as IEnumerable<object>);

                if (!string.IsNullOrEmpty(itemSourceEventUpdateName))
                {
                    EventInfo eventInfo = type.GetEvent(itemSourceEventUpdateName);
                    Action update = () => Notify(nameof(Items));

                    eventInfo.AddMethod?.Invoke(source, new object[] { update });
                }
            }

            Placeholder = placeholder;
        }

        public string Placeholder { get; }
        public ObservableCollection<object> Items => GetSource?.Invoke();
    }

    public class EnumComboBoxContext : VisualTypeContext
    {
        public EnumComboBoxContext(string tag, string placeholder, object source, string property, IEnumerable<string> items, bool? hideTag = null)
            : base(tag, source, property, new ComboBoxControl(hideTag ?? string.IsNullOrEmpty(tag), null))
        {
            Placeholder = placeholder;
            Items = new ObservableCollection<object>(items);
        }

        public string Placeholder { get; }
        public ObservableCollection<object> Items { get; }

        protected override object GetValue() =>  base.GetValue();
    }
}
