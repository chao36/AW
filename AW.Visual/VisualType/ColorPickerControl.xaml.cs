using System.Windows;
using System.Windows.Controls;

using AW.Visual.Common;

using MaterialDesignThemes.Wpf;

using MColor = System.Windows.Media.Color;

namespace AW.Visual.VisualType
{
    public partial class ColorPickerControl : UserControl
    {
        public ColorPickerControl(bool hideTag)
        {
            InitializeComponent();

            if (hideTag)
                TagLabel.Visibility = Visibility.Collapsed;
        }
    }

    public class ColorPickerContext : VisualTypeContext
    {
        public ColorPickerContext(string tag, object source, string property, bool? hideTag = null)
            : base(tag, source, property, new ColorPickerControl(hideTag ?? string.IsNullOrEmpty(tag)))
        {
            Ok = new ActionContext(AWWindow.OkTitle, PackIconKind.Check, () =>
            {
                SetValue((Control as ColorPickerControl).ColorPicker.CurrentColor);
                (Control as ColorPickerControl).Element.IsPopupOpen = false;
            }, fontSize: 12, iconSize: 20);
            Cancel = new ActionContext(AWWindow.CancelTitle, PackIconKind.Close, () =>
            {
                (Control as ColorPickerControl).Element.IsPopupOpen = false;
            }, fontSize: 12, iconSize: 20, iconColor: ColorHelper.RedSet.Color500.ToBrush());
        }

        public ActionContext Ok { get; }
        public ActionContext Cancel { get; }

        protected override object GetValue()
        {
            MColor value = (MColor)base.GetValue();

            (Control as ColorPickerControl).ColorPicker.CurrentColor = value;
            return value;
        }
    }
}
