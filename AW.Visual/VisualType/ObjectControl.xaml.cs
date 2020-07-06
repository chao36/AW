using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using AW.Base;
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
                VisualHelper.LeftDown(ItemContainer, _ =>
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
        private static readonly Type ColorType = typeof(Color);
        private static readonly Type ActionType = typeof(Action);
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
                        AWPropertyAttribute attribute = p.GetCustomAttribute<AWPropertyAttribute>();
                        string tag = attribute.Tag ?? p.Name;

                        if (attribute is AWComboBoxAttribute comboBoxAttribute)
                            return new ComboBoxContext(tag, null, source, p.Name, comboBoxAttribute, true);

                        if (p.PropertyType == IntType)
                            return new TextBoxContext(tag, null, source, p.Name, TextBoxType.Int, attribute, true);

                        if (p.PropertyType == DoubleType)
                            return new TextBoxContext(tag, null, source, p.Name, TextBoxType.Double, attribute, true);

                        if (p.PropertyType == StringType)
                            return attribute is AWReadonlyAttribute
                                ? (IVisualTypeContext)new LabelContext(tag, source, p.Name)
                                : new TextBoxContext(tag, null, source, p.Name, TextBoxType.String, attribute, true);

                        if (p.PropertyType == BoolType)
                            return new CheckBoxContext(tag, source, p.Name, true);

                        if (p.PropertyType == DateTimeType)
                            return new DateContext(tag, null, source, p.Name, true);

                        if (p.PropertyType == ColorType)
                            return new ColorPickerContext(tag, source, p.Name, true);

                        if (p.PropertyType == ActionType)
                            return new ButtonContext(tag, source, p.Name, attribute as AWActionAttribute);

                        if (p.PropertyType.IsEnum)
                            return new EnumComboBoxContext(tag, null, source, p.Name, Enum.GetNames(p.PropertyType), true);

                        if (p.PropertyType.GetInterfaces().Any(t => t == EnumerableType) && p.PropertyType.GenericTypeArguments[0].IsClass && p.PropertyType.GenericTypeArguments[0] != StringType)
                            return new CollectionContext(tag, source, p.Name, Left + 20);

                        if (p.PropertyType.IsClass)
                            return new ObjectContext(tag, source, p.Name, Left + 20);

                        return null;
                    })
                    .Where(c => c != null);

                Properties = new ObservableCollection<IVisualTypeContext>(properties);
            }
        }
    }
}
