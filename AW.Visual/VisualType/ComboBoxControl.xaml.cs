using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;

using AW.Base;
using AW.Visual.Common;

namespace AW.Visual.VisualType
{
    public partial class ComboBoxControl : BaseControl
    {
        public ComboBoxControl(bool hideTag, string displayMemberPath)
        {
            InitializeComponent();

            if (hideTag)
                TagLabel.Visibility = Visibility.Collapsed;

            Element.DisplayMemberPath = displayMemberPath;
        }

        protected override void OnDataContextChange()
        {
            string style = null;

            if (DataContext is ComboBoxContext comboBoxContext)
                style = comboBoxContext.Style;
            if (DataContext is EnumComboBoxContext enumComboBoxContext)
                style = enumComboBoxContext.Style;
            
            if (!string.IsNullOrEmpty(style))
                Element.SetResourceReference(StyleProperty, FindResource(style));
        }

        protected override void OnLoaded()
        {
            if (DataContext is VisualTypeContext context)
                context.Notify(nameof(context.Value));
        }
    }

    public class ComboBoxContext : VisualTypeContext
    {
        private readonly Func<ObservableCollection<object>> GetSource;

        public ComboBoxContext(string tag, string placeholder, object source, string property, AWComboBoxAttribute attribute, bool? hideTag = null)
            : base(tag, source, property, new ComboBoxControl(hideTag ?? string.IsNullOrEmpty(tag), attribute.DisplayMemberPath))
        {
            if (!string.IsNullOrEmpty(attribute.SourceName))
            {
                Type type = source.GetType();

                PropertyInfo propertyInfo = type.GetProperty(attribute.SourceName);

                if (propertyInfo != null)
                    GetSource = () => new ObservableCollection<object>(propertyInfo.GetValue(source) as IEnumerable<object>);

                if (!string.IsNullOrEmpty(attribute.UpdateSourceEventName))
                {
                    EventInfo eventInfo = type.GetEvent(attribute.UpdateSourceEventName);
                    Action update = () => Notify(nameof(Items));

                    eventInfo.AddMethod?.Invoke(source, new object[] { update });
                }
            }

            Placeholder = placeholder;
        }

        public string Style { get; set; }

        public string Placeholder { get; }
        public ObservableCollection<object> Items => GetSource?.Invoke();
    }

    public class EnumComboBoxContext : VisualTypeContext
    {
        public EnumComboBoxContext(string tag, string placeholder, object source, string property, IEnumerable<string> items, bool? hideTag = null)
            : base(tag, source, property, new ComboBoxControl(hideTag ?? string.IsNullOrEmpty(tag), null))
        {
            Placeholder = placeholder;
            Items = new ObservableCollection<string>(items);
        }

        public string Style { get; set; }

        public string Placeholder { get; }
        public ObservableCollection<string> Items { get; }

        public override object Value { get => base.Value.ToString(); set => base.Value = value; }
    }
}
