using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using AW.Base.Serializer.Common;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.VisualType
{
    public partial class ObjectControl : UserControl
    {
        private bool IsOpen { get; set; }

        public ObjectControl(bool isTop, int left = 0)
        {
            InitializeComponent();

            if (isTop)
            {
                ItemContainer.Visibility = Visibility.Collapsed;
                SubItemContainer.Visibility = Visibility.Visible;
            }
            else
            {
                ClickHelper.LeftDown(ItemContainer, _ =>
                {
                    IsOpen = !IsOpen;

                    UpdateGroup(IsOpen);
                });

                SubItemContainer.Margin = new Thickness(left, 0, 0, 0);
            }
        }

        private void UpdateGroup(bool value)
        {
            if (value)
            {
                Group.Kind = PackIconKind.MenuDown;
                SubItemContainer.Visibility = Visibility.Visible;
            }
            else
            {
                Group.Kind = PackIconKind.MenuRight;
                SubItemContainer.Visibility = Visibility.Collapsed;
            }
        }
    }

    public class ObjectContext : VisualTypeContext
    {
        public ObjectContext(object source) : base(null, source, null, new ObjectControl(true))
            => SetProperties(Source);

        public ObjectContext(string tag, object source, int left) : base(tag, source, null, new ObjectControl(false, left))
        {
            Left = left;

            SetProperties(source);
        }

        public ObjectContext(string tag, object source, string property, int left) : base(tag, source, property, new ObjectControl(false, left))
        {
            Left = left;

            source = GetValue();
            if (source == null)
                SetValue(SerializerHelper.GetObject(Source.GetType().GetProperty(PropertyName).PropertyType));

            SetProperties(source ?? GetValue());
        }

        public override object Value { get => Properties; set { } }
        private ObservableCollection<IVisualTypeContext> Properties { get; set; } = new ObservableCollection<IVisualTypeContext>();

        private static readonly Type IntType = typeof(int);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type StringType = typeof(string);
        private static readonly Type BoolType = typeof(bool);
        private static readonly Type DateTimeType = typeof(DateTime);
        private static readonly Type EnumerableType = typeof(IList);

        private void SetProperties(object source)
        {
            if (source == null)
                return;

            Type type = source.GetType();

            if (type != null && type.IsClass)
            {
                IEnumerable<IVisualTypeContext> properties = type.GetProperties()
                    .Where(p => p.GetCustomAttribute<AWPropertyAttribute>() != null)
                    .OrderBy(p => p.GetCustomAttribute<AWPropertyAttribute>()?.Index ?? 0)
                    .Select(p =>
                    {
                        if (p.PropertyType == IntType)
                            return new TextBoxContext(p.Name, null, source, p.Name, TextBoxType.Int, true);

                        if (p.PropertyType == DoubleType)
                            return new TextBoxContext(p.Name, null, source, p.Name, TextBoxType.Double, true);

                        if (p.PropertyType == StringType)
                            return p.GetCustomAttribute<AWReadonlyAttribute>() != null
                                ? new LabelContext(p.Name, source, p.Name)
                                : (IVisualTypeContext)new TextBoxContext(p.Name, null, source, p.Name, TextBoxType.String, true);

                        if (p.PropertyType == BoolType)
                            return new CheckBoxContext(p.Name, source, p.Name, true);

                        if (p.PropertyType == DateTimeType)
                            return new DateContext(p.Name, null, source, p.Name, true);

                        if (p.PropertyType.IsEnum)
                            return new ComboBoxContext(p.Name, null, source, p.Name, null, Enum.GetNames(p.PropertyType), true);

                        if (p.PropertyType.GetInterfaces().Any(t => t == EnumerableType))
                            return new CollectionContext(p.Name, source, p.Name, Left + 25);

                        if (p.PropertyType.IsClass)
                            return new ObjectContext(p.Name, source, p.Name, Left + 25);

                        return null;
                    })
                    .Where(c => c != null);

                Properties = new ObservableCollection<IVisualTypeContext>(properties);
            }
        }
    }
}
