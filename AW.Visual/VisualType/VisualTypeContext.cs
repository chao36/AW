using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

using AW.Visual.Common;

namespace AW.Visual.VisualType
{
    public interface IVisualTypeContext : INotifyPropertyChanged
    {
        int Left { get; set; }

        object Source { get; }
        string PropertyName { get; }

        string Tag { get; }
        object Value { get; set; }

        FrameworkElement Control { get; }
    }

    public class VisualTypeContext : BaseContext, IVisualTypeContext
    {
        public int Left { get; set; }

        public VisualTypeContext(string tag, object source, string property, FrameworkElement control)
        {
            Source = source;
            PropertyName = property;

            Tag = tag;

            ControlSource = control;

            if (Source is INotifyPropertyChanged notify)
                notify.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == PropertyName)
                        Notify(nameof(Value));
                };
        }

        public object Source { get; }
        public string PropertyName { get; }

        public virtual string Tag { get; }
        public virtual object Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        private readonly FrameworkElement ControlSource;
        public FrameworkElement Control
        {
            get
            {
                if (ControlSource.DataContext != this)
                    ControlSource.DataContext = this;

                return ControlSource;
            }
        }

        protected virtual object GetValue()
        {
            if (Source != null)
            {
                PropertyInfo property = Source.GetType().GetProperty(PropertyName);

                if (property != null && property.CanRead)
                    return property.GetValue(Source);
            }

            return null;
        }

        protected virtual void SetValue(object value)
        {
            if (Source != null)
            {
                PropertyInfo property = Source.GetType().GetProperty(PropertyName);

                value = property.PropertyType.IsEnum
                    ? Enum.Parse(property.PropertyType, value.ToString())
                    : Convert.ChangeType(value, property.PropertyType);

                if (property != null && property.CanWrite)
                    property.SetValue(Source, value);
            }

            Notify(nameof(Value));
        }
    }
}
