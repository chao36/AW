using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AW.Visual.VisualType
{
    public partial class TextBoxControl : UserControl
    {
        public TextBoxControl(bool hideTag, TextBoxType textBoxType)
        {
            InitializeComponent();

            if (hideTag)
                Tag.Visibility = Visibility.Collapsed;

            if (textBoxType != TextBoxType.Text)
            {
                Func<string, bool> canChange = textBoxType == TextBoxType.Double
                    ? (Func<string, bool>)(v => !double.TryParse(v, out double _))
                    : (v => !int.TryParse(v, out int _));

                Element.PreviewTextInput += (s, e) =>
                {
                    e.Handled = !string.IsNullOrWhiteSpace(e.Text)
                        && (e.Text != "-" || Element.CaretIndex != 0)
                        && canChange(Element.Text.Insert(Element.CaretIndex, e.Text));
                };
                DataObject.AddPastingHandler(Element, (s, e) =>
                {
                    if (e.DataObject.GetDataPresent(typeof(string)))
                    {
                        string text = (string)e.DataObject.GetData(typeof(string));

                        if (canChange(Element.Text.Insert(Element.CaretIndex, text)))
                            e.CancelCommand();
                    }
                    else
                        e.CancelCommand();
                });
            }

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

                    BindingExpression be = Element.GetBindingExpression(TextBox.TextProperty);
                    if (be != null)
                        be.UpdateSource();
                }
            };
        }
    }

    public enum TextBoxType
    {
        Text = 0,
        Double = 1,
        Int = 2
    }

    public class TextBoxContext : VisualTypeContext
    {
        public TextBoxContext(string tag, string placeholder, object source, string property, TextBoxType textBoxType)
            : base(tag, source, property, new TextBoxControl(string.IsNullOrEmpty(tag), textBoxType))
            => Placeholder = placeholder;

        public string Placeholder { get; }
    }
}