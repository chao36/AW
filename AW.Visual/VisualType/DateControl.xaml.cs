using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AW.Visual.VisualType
{
    public partial class DateControl : UserControl
    {
        public DateControl(bool hideTag)
        {
            InitializeComponent();

            if (hideTag)
                TagLabel.Visibility = Visibility.Collapsed;

            Element.PreviewKeyUp += (s, e) =>
            {
                if (e.Key == Key.Return)
                {
                    DependencyObject ancestor = Element.Parent;
                    while (ancestor != null)
                    {
                        if (ancestor is UIElement element && element.Focusable)
                        {
                            element.Focus();
                            break;
                        }

                        ancestor = VisualTreeHelper.GetParent(ancestor);
                    }
                    Keyboard.ClearFocus();

                    BindingExpression be = Element.GetBindingExpression(DatePicker.SelectedDateProperty);
                    if (be != null)
                        be.UpdateSource();
                }
            };
        }
    }

    public class DateContext : VisualTypeContext
    {
        public DateContext(string tag, string placeholder, object source, string property, bool? hideTag = null)
            : base(tag, source, property, new DateControl(hideTag ?? string.IsNullOrEmpty(tag)))
            => Placeholder = placeholder;

        public string Placeholder { get; }
    }
}
