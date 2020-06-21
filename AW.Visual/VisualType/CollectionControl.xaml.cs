using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using AW.Base.Serializer.Common;
using AW.Visual;
using AW.Visual.Common;
using AW.Visual.VisualType;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.VisualType
{
    public partial class CollectionControl : UserControl
    {
        private bool IsOpen { get; set; }

        public CollectionControl(int left = 0)
        {
            InitializeComponent();

            ClickHelper.LeftDown(ItemContainer, _ =>
            {
                IsOpen = !IsOpen;

                UpdateGroup(IsOpen);
            });

            SubItemContainer.Margin = new Thickness(left, 0, 0, 0);
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
}

public class CollectionContext : VisualTypeContext
{
    public CollectionContext(string tag, object source, string property, int left) : base(tag, source, property, new CollectionControl(left))
    {
        Left = left;
        source = GetValue();

        if (source == null)
            SetValue(SerializerHelper.GetObject(Source.GetType().GetProperty(PropertyName).PropertyType));

        SetProperties((source ?? GetValue()) as IList);
    }

    public override object Value { get => Properties; set { } }
    private ObservableCollection<IVisualTypeContext> Properties { get; set; } = new ObservableCollection<IVisualTypeContext>();

    private void SetProperties(IList source)
    {
        if (source == null)
            return;

        List<IVisualTypeContext> properties = new List<IVisualTypeContext>();

        for (int i = 0; i < source.Count; ++i)
            properties.Add(new ObjectContext($"[{i}]", source[i], Left + 25));

        Properties = new ObservableCollection<IVisualTypeContext>(properties);

        Type itemType = source.GetType().GenericTypeArguments[0];

        Add = new ActionContext(AWWindow.AddTitle, PackIconKind.Add, () =>
        {
            source.Add(SerializerHelper.GetObject(itemType));

            int index = source.Count - 1;
            Properties.Add(new ObjectContext($"[{index}]", source[index], Left + 25));
        }, fontSize: 12, iconSize: 20);

        Remove = new ActionContext(AWWindow.RemoveTitle, PackIconKind.Close, () =>
        {
            int index = source.Count - 1;

            source.RemoveAt(index);
            Properties.RemoveAt(index);
        }, fontSize: 12, iconSize: 20, iconColor: ColorHelper.RedSet.Color500.ToBrush());
    }

    public ActionContext Add { get; set; }
    public ActionContext Remove { get; set; }
}
