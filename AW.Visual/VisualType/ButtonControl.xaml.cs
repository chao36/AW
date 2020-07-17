using System;
using System.Reflection;
using System.Windows.Input;

using AW.Base;
using AW.Visual.Common;

namespace AW.Visual.VisualType
{
    public partial class ButtonControl : BaseControl
    {
        public ButtonControl() => InitializeComponent();

        protected override void OnDataContextChange()
        {
            if (DataContext is ButtonContext context && !string.IsNullOrEmpty(context.Style))
                Element.SetResourceReference(StyleProperty, FindResource(context.Style));
        }
    }

    public class ButtonContext : VisualTypeContext
    {
        public ButtonContext(string tag, object source, string property, AWActionAttribute attribute = null)
            : base(tag, source, property, new ButtonControl())
        {
            Content = attribute?.Content ?? tag;

            Action action = (Action)GetValue();
            Func<bool> canAction = null;

            if (!string.IsNullOrEmpty(attribute?.CanExecuteName))
            {
                Type type = source.GetType();

                PropertyInfo propertyInfo = type.GetProperty(attribute.CanExecuteName);

                if (propertyInfo != null)
                    canAction = (Func<bool>)propertyInfo.GetValue(source);
            }

            Command = new SimpleCommand(action, canAction);
        }

        public string Style { get; set; }

        public string Content { get; }
        public ICommand Command { get; }
    }
}
