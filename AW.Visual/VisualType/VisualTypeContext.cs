using AW.Visual.Common;

using System;
using System.Reflection;
using System.Windows;

namespace AW.Visual.VisualType
{
    public interface IVisualTypeContext
    {
        object Source { get; }
        string PropertyName { get; }

        string Tag { get; }
        object Value { get; set; }

        FrameworkElement Control { get; }
    }

    public class VisualTypeContext : BaseContext, IVisualTypeContext
    {
        public VisualTypeContext(string tag, object source, string property, FrameworkElement control)
        {
            Source = source;
            PropertyName = property;

            Tag = tag;

            Control = control;
            Control.DataContext = this;
        }

        public object Source { get; }
        public string PropertyName { get; }

        public string Tag { get; }
        public virtual object Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public FrameworkElement Control { get; }

        protected object GetValue()
        {
            if (Source != null)
            {
                PropertyInfo property = Source.GetType().GetProperty(PropertyName);

                if (property != null && property.CanRead)
                    return property.GetValue(Source);
            }

            return null;
        }

        protected void SetValue(object value)
        {
            if (Source != null)
            {
                PropertyInfo property = Source.GetType().GetProperty(PropertyName);

                if (property != null && property.CanWrite)
                    property.SetValue(Source, Convert.ChangeType(value, property.PropertyType));
            }

            Notify(nameof(Value));
        }
    }
}
