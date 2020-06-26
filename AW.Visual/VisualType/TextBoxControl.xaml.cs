using System.Windows;
using System.Windows.Controls;

namespace AW.Visual.VisualType
{
    public partial class TextBoxControl : UserControl
    {
        public TextBoxControl(bool hideTag, TextBoxType textBoxType)
        {
            InitializeComponent();

            if (hideTag)
                TagLabel.Visibility = Visibility.Collapsed;

            if (textBoxType != TextBoxType.String)
                VisualHelper.LimitInput(Element, textBoxType == TextBoxType.Double ? LimitType.Double : LimitType.Int);

            VisualHelper.ExitOnEnter(Element);
        }
    }

    public enum TextBoxType
    {
        String = 0,
        Double = 1,
        Int = 2
    }

    public class TextBoxContext : VisualTypeContext
    {
        public TextBoxContext(string tag, string placeholder, object source, string property, TextBoxType textBoxType, bool? hideTag = null)
            : base(tag, source, property, new TextBoxControl(hideTag ?? string.IsNullOrEmpty(tag), textBoxType))
            => Placeholder = placeholder;

        public string Placeholder { get; }
    }
}